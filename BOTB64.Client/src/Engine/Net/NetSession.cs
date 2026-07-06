using BOTB64.Entities;
using BOTB64.Shared;
using LiteNetLib;
using MessagePack;
using System.Collections.Concurrent;

namespace BOTB64.Engine.Net
{
    public class Player
    {
        public int PlayerID;
        public string DisplayName = "Player";
        public Faction Faction;
        public List<int> OwnedPickSlots = new();
        public bool IsConnected = true;
    }

    public class NetSession
    {
        public Guid MatchID;
        public int LocalPlayerID;
        public int HostPlayerID;
        public bool IsHost => LocalPlayerID == HostPlayerID;
        public List<Player> Players = new();
        public Player LocalPlayer => Players.First(p => p.PlayerID == LocalPlayerID);

        private NetManager? NetManager;
        private NetPeer? ServerPeer;
        private readonly EventBasedNetListener Listener = new();
        private CancellationTokenSource? PollCts;

        private readonly ConcurrentQueue<Action> MainThreadActions = new();

        public NetSession(Guid matchId, int localPlayerId, int hostPlayerId)
        {
            MatchID = matchId;
            LocalPlayerID = localPlayerId;
            HostPlayerID = hostPlayerId;
        }

        public Task Connect(string relayHost, int relayPort, string connectionKey = "botb64", int timeoutMs = 5000)
        {
            var tcs = new TaskCompletionSource();

            Listener.NetworkReceiveEvent += (peer, reader, channel, method) =>
            {
                try
                {
                    var envelope = MessagePackSerializer.Deserialize<RelayEnvelope>(reader.GetRemainingBytes());
                    OnEnvelopeReceived(envelope);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"NetSession receive error: {ex}");
                }
                finally
                {
                    reader.Recycle();
                }
            };

            Listener.PeerConnectedEvent += peer =>
            {
                ServerPeer = peer;
                Send(new RelayEnvelope
                {
                    MatchID = MatchID,
                    Type = RelayMessageType.Join,
                    SenderID = LocalPlayerID,
                    TargetID = -1,
                    Payload = Array.Empty<byte>()
                });
                tcs.TrySetResult();
            };

            Listener.PeerDisconnectedEvent += (peer, info) =>
                Console.WriteLine($"Disconnected from relay: {info.Reason}");

            NetManager = new NetManager(Listener) { DisconnectTimeout = 15000 };
            NetManager.Start();
            NetManager.Connect(relayHost, relayPort, connectionKey);

            PollCts = new CancellationTokenSource();
            _ = PollLoop(PollCts.Token);

            return Task.WhenAny(tcs.Task, Task.Delay(timeoutMs))
                .ContinueWith(t =>
                {
                    if (!tcs.Task.IsCompleted)
                        throw new TimeoutException($"Could not connect to relay at {relayHost}:{relayPort}");
                });
        }

        public void Disconnect()
        {
            PollCts?.Cancel();
            NetManager?.Stop();
        }

        private async Task PollLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested && NetManager is { IsRunning: true })
            {
                NetManager.PollEvents(); // this thread invokes NetworkReceiveEvent -> OnEnvelopeReceived -> just enqueues now
                try { await Task.Delay(15, token); } catch (TaskCanceledException) { }
            }
        }

        public void PumpMainThreadActions()
        {
            while (MainThreadActions.TryDequeue(out var action))
                action();
        }

        public void SendCommand(IGameCommand command) => Send(new RelayEnvelope
        {
            MatchID = MatchID,
            Type = RelayMessageType.Command,
            SenderID = LocalPlayerID,
            TargetID = HostPlayerID,
            Payload = MessagePackSerializer.Serialize(command)
        });

        public void BroadcastEvents(List<IGameEvent> events) => Send(new RelayEnvelope
        {
            MatchID = MatchID,
            Type = RelayMessageType.EventBatch,
            SenderID = LocalPlayerID,
            TargetID = -1,
            Payload = MessagePackSerializer.Serialize(events)
        });

        public void SendPickCommand(PickCommand command) => Send(new RelayEnvelope
        {
            MatchID = MatchID,
            Type = RelayMessageType.PickCommand,
            SenderID = LocalPlayerID,
            TargetID = HostPlayerID,
            Payload = MessagePackSerializer.Serialize(command)
        });

        public void BroadcastPickEvent(PickEvent evt) => Send(new RelayEnvelope
        {
            MatchID = MatchID,
            Type = RelayMessageType.PickEvent,
            SenderID = LocalPlayerID,
            TargetID = -1,
            Payload = MessagePackSerializer.Serialize(evt)
        });

        public void BroadcastMatchStart(GameInitializer init) => Send(new RelayEnvelope
        {
            MatchID = MatchID,
            Type = RelayMessageType.MatchStart,
            SenderID = LocalPlayerID,
            TargetID = -1,
            Payload = MessagePackSerializer.Serialize(init)
        });

        private void Send(RelayEnvelope envelope)
        {
            Console.WriteLine($"[{LocalPlayerID}] SEND type={envelope.Type} peerState={ServerPeer?.ConnectionState}");
            if (ServerPeer == null || ServerPeer.ConnectionState != ConnectionState.Connected)
            {
                Console.WriteLine($"Send skipped — ServerPeer state: {ServerPeer?.ConnectionState.ToString() ?? "null"}");
                return;
            }
            ServerPeer.Send(MessagePackSerializer.Serialize(envelope), DeliveryMethod.ReliableOrdered);
        }

        public void OnEnvelopeReceived(RelayEnvelope envelope)
        {
            Console.WriteLine($"[{LocalPlayerID}] RECV type={envelope.Type}");
            switch (envelope.Type)
            {
                case RelayMessageType.Command:
                    var cmd = MessagePackSerializer.Deserialize<IGameCommand>(envelope.Payload);
                    MainThreadActions.Enqueue(() => OnCommandReceived?.Invoke(cmd));
                    break;
                case RelayMessageType.EventBatch:
                    var events = MessagePackSerializer.Deserialize<List<IGameEvent>>(envelope.Payload);
                    MainThreadActions.Enqueue(() => OnEventsReceived?.Invoke(events));
                    break;
                case RelayMessageType.HostMigrated:
                    var newHost = MessagePackSerializer.Deserialize<int>(envelope.Payload);
                    MainThreadActions.Enqueue(() =>
                    {
                        HostPlayerID = newHost;
                        OnHostMigrated?.Invoke(newHost);
                    });
                    break;
                case RelayMessageType.PlayerDisconnected:
                    var disconnectedId = MessagePackSerializer.Deserialize<int>(envelope.Payload);
                    MainThreadActions.Enqueue(() =>
                    {
                        var p = Players.FirstOrDefault(pl => pl.PlayerID == disconnectedId);
                        if (p != null) p.IsConnected = false;
                    });
                    break;
                case RelayMessageType.PickCommand:
                    var pickCmd = MessagePackSerializer.Deserialize<PickCommand>(envelope.Payload);
                    MainThreadActions.Enqueue(() => OnPickCommandReceived?.Invoke(pickCmd));
                    break;
                case RelayMessageType.PickEvent:
                    var pickEvt = MessagePackSerializer.Deserialize<PickEvent>(envelope.Payload);
                    MainThreadActions.Enqueue(() => OnPickEventReceived?.Invoke(pickEvt));
                    break;
                case RelayMessageType.MatchStart:
                    var init = MessagePackSerializer.Deserialize<GameInitializer>(envelope.Payload);
                    MainThreadActions.Enqueue(() => OnMatchStartReceived?.Invoke(init));
                    break;
            }
        }

        public event Action<IGameCommand>? OnCommandReceived;
        public event Action<List<IGameEvent>>? OnEventsReceived;
        public event Action<int>? OnHostMigrated;
        public event Action<PickCommand>? OnPickCommandReceived;
        public event Action<PickEvent>? OnPickEventReceived;
        public event Action<GameInitializer>? OnMatchStartReceived;
    }
}