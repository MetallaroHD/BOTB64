using BOTB64.Entities;
using BOTB64.Shared;
using LiteNetLib;
using MessagePack;
using System.Text.RegularExpressions;

namespace BOTB64.Engine.Net
{
    public class Player
    {
        public int PlayerID;
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
        
        private NetPeer ServerPeer;

        public NetSession(Guid matchId, int localPlayerId, int hostPlayerId)
        {
            MatchID = matchId;
            LocalPlayerID = localPlayerId;
            HostPlayerID = hostPlayerId;
        }

        public void SendCommand(IGameCommand command)
        {
            Send(new RelayEnvelope
            {
                MatchID = MatchID,
                Type = RelayMessageType.Command,
                SenderID = LocalPlayerID,
                TargetID = HostPlayerID,
                Payload = MessagePackSerializer.Serialize(command)
            });
        }

        public void BroadcastEvents(List<IGameEvent> events)
        {
            Send(new RelayEnvelope
            {
                MatchID = MatchID,
                Type = RelayMessageType.EventBatch,
                SenderID = LocalPlayerID,
                TargetID = -1, //everyone
                Payload = MessagePackSerializer.Serialize(events)
            });
        }

        private void Send(RelayEnvelope envelope) =>
            ServerPeer.Send(MessagePackSerializer.Serialize(envelope), DeliveryMethod.ReliableOrdered);

        // called from the NetManager's receive callback
        public void OnEnvelopeReceived(RelayEnvelope envelope)
        {
            switch (envelope.Type)
            {
                case RelayMessageType.Command:
                    var cmd = MessagePackSerializer.Deserialize<IGameCommand>(envelope.Payload);
                    OnCommandReceived?.Invoke(cmd); // host-side handler, see NetworkedCommandChannel below
                    break;
                case RelayMessageType.EventBatch:
                    var events = MessagePackSerializer.Deserialize<List<IGameEvent>>(envelope.Payload);
                    OnEventsReceived?.Invoke(events);
                    break;
                case RelayMessageType.HostMigrated:
                    HostPlayerID = MessagePackSerializer.Deserialize<int>(envelope.Payload);
                    OnHostMigrated?.Invoke(HostPlayerID);
                    break;
                case RelayMessageType.PlayerDisconnected:
                    var disconnectedId = MessagePackSerializer.Deserialize<int>(envelope.Payload);
                    Players.First(p => p.PlayerID == disconnectedId).IsConnected = false;
                    break;
                case RelayMessageType.PickCommand:
                    var pickCmd = MessagePackSerializer.Deserialize<PickCommand>(envelope.Payload);
                    OnPickCommandReceived?.Invoke(pickCmd);
                    break;
                case RelayMessageType.PickEvent:
                    var pickEvt = MessagePackSerializer.Deserialize<PickEvent>(envelope.Payload);
                    OnPickEventReceived?.Invoke(pickEvt);
                    break;
            }
        }

        public void SendPickCommand(PickCommand command)
        {
            Send(new RelayEnvelope
            {
                MatchID = MatchID,
                Type = RelayMessageType.PickCommand,
                SenderID = LocalPlayerID,
                TargetID = HostPlayerID,
                Payload = MessagePackSerializer.Serialize(command)
            });
        }

        public void BroadcastPickEvent(PickEvent evt)
        {
            Send(new RelayEnvelope
            {
                MatchID = MatchID,
                Type = RelayMessageType.PickEvent,
                SenderID = LocalPlayerID,
                TargetID = -1,
                Payload = MessagePackSerializer.Serialize(evt)
            });
        }

        public event Action<IGameCommand>? OnCommandReceived;
        public event Action<List<IGameEvent>>? OnEventsReceived;
        public event Action<int>? OnHostMigrated;
        public event Action<PickCommand>? OnPickCommandReceived;
        public event Action<PickEvent>? OnPickEventReceived;
    }
}
