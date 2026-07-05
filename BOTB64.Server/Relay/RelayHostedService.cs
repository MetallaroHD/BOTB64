using BOTB64.Server.Lobbies;
using BOTB64.Shared;
using LiteNetLib;
using MessagePack;

namespace BOTB64.Server.Relay
{
    public class RelayHostedService : BackgroundService
    {
        private readonly MatchRelay MatchRelay;
        private readonly LobbyManager LobbyManager;
        private readonly EventBasedNetListener Listener = new();
        private NetManager? NetManager;
        private const int RelayPort = 9050;

        public RelayHostedService(MatchRelay matchRelay, LobbyManager lobbyManager)
        {
            MatchRelay = matchRelay;
            LobbyManager = lobbyManager;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Listener.ConnectionRequestEvent += request => request.AcceptIfKey("botb64"); // simple shared key, swap for real auth later
            Listener.PeerConnectedEvent += peer => Console.WriteLine($"Peer connected: {peer}");

            Listener.NetworkReceiveEvent += (peer, reader, channel, method) =>
            {
                try
                {
                    var bytes = reader.GetRemainingBytes();
                    var envelope = MessagePackSerializer.Deserialize<RelayEnvelope>(bytes);
                    
                    if (envelope.Type == RelayMessageType.Join)
                    {
                        var lobby = LobbyManager.FindByMatchID(envelope.MatchID);
                        int hostId = lobby?.Players.First(p => p.IsHost).PlayerId ?? envelope.SenderID;
                        MatchRelay.HandleJoin(envelope.MatchID, envelope.SenderID, peer, hostId);
                    }
                    else
                    {
                        MatchRelay.Route(envelope);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Relay receive error: {ex.Message}");
                }
                finally
                {
                    reader.Recycle();
                }
            };

            Listener.PeerDisconnectedEvent += (peer, info) => MatchRelay.HandleDisconnectByPeer(peer);

            NetManager = new NetManager(Listener) { DisconnectTimeout = 15000 };
            NetManager.Start(RelayPort);
            Console.WriteLine($"Relay listening on UDP {RelayPort}");

            while (!stoppingToken.IsCancellationRequested)
            {
                NetManager.PollEvents();
                await Task.Delay(15, stoppingToken);
            }

            NetManager.Stop();
        }
    }
}
