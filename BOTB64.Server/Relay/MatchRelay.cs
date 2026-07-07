using BOTB64.Shared;
using LiteNetLib;
using MessagePack;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.Hosting;

namespace BOTB64.Server.Relay
{
    public class MatchConnection
    {
        public required int PlayerId;
        public required NetPeer Peer;
    }

    public class ActiveMatch
    {
        public Guid MatchId;
        public int HostPlayerId;
        public List<MatchConnection> Connections = new();
    }

    public class MatchRelay
    {
        private readonly Dictionary<Guid, ActiveMatch> Matches = new();
        private readonly object Lock = new();

        // Called once a lobby is full and clients start connecting to the relay for that match
        public void RegisterConnection(Guid matchId, int playerId, NetPeer peer, int initialHostPlayerId)
        {
            lock (Lock)
            {
                if (!Matches.TryGetValue(matchId, out var match))
                {
                    match = new ActiveMatch { MatchId = matchId, HostPlayerId = initialHostPlayerId };
                    Matches[matchId] = match;
                }
                match.Connections.Add(new MatchConnection { PlayerId = playerId, Peer = peer });
            }
        }

        public void Route(RelayEnvelope envelope)
        {
            lock (Lock)
            {
                if (!Matches.TryGetValue(envelope.MatchID, out var match)) return;

                // Commands only ever go to the current host, regardless of what TargetPlayerId says —
                // server enforces this rather than trusting the sender's routing.
                if (envelope.Type == RelayMessageType.Command || envelope.Type == RelayMessageType.PickCommand)
                {
                    var host = match.Connections.FirstOrDefault(c => c.PlayerId == match.HostPlayerId);
                    if (host == null)
                    {
                        Console.WriteLine($"Route: no host found for match {envelope.MatchID}, expected host {match.HostPlayerId}");
                    }
                    else
                    {
                        host.Peer.Send(Serialize(envelope), DeliveryMethod.ReliableOrdered);
                        Console.WriteLine($"[SERVER] ROUTE type={envelope.Type} -> target(s)={string.Join(",", host.Peer.Address)}");
                    }
                    return;
                }

                // MatchStart is a broadcast that must include the sender too — unlike EventBatch/PickEvent,
                // the host doesn't apply this one locally before sending; it waits for the same
                // OnMatchStartReceived echo as every other client, so everyone (host included) needs a copy.
                if (envelope.Type == RelayMessageType.MatchStart)
                {
                    foreach (var conn in match.Connections)
                    {
                        Console.WriteLine($"[SERVER] ROUTE type={envelope.Type} -> target(s)={string.Join(",", conn.Peer.Address)}");
                        conn.Peer.Send(Serialize(envelope), DeliveryMethod.ReliableOrdered);
                    }
                    return;
                }

                // Broadcasts (event batches, host-migrated notices) go to everyone except the sender
                foreach (var conn in match.Connections)
                {
                    if (conn.PlayerId == envelope.SenderID) continue;
                    Console.WriteLine($"[SERVER] ROUTE type={envelope.Type} -> target(s)={string.Join(",", conn.Peer.Address)}");
                    conn.Peer.Send(Serialize(envelope), DeliveryMethod.ReliableOrdered);
                }
            }
        }

        public void HandleDisconnect(Guid matchId, int playerId)
        {
            lock (Lock)
            {
                if (!Matches.TryGetValue(matchId, out var match)) return;
                match.Connections.RemoveAll(c => c.PlayerId == playerId);

                if (match.Connections.Count == 0)
                {
                    Matches.Remove(matchId);
                    return;
                }

                bool wasHost = match.HostPlayerId == playerId;
                match.HostPlayerId = match.Connections.OrderBy(c => c.PlayerId).First().PlayerId;
                var notice = new RelayEnvelope
                {
                    MatchID = matchId,
                    SenderID = -1,
                    TargetID = -1,
                    Type = RelayMessageType.PlayerDisconnected,
                    Payload = Serialize(playerId)
                };
                Broadcast(match, notice);

                if (wasHost)
                {
                    match.HostPlayerId = match.Connections.OrderBy(c => c.PlayerId).First().PlayerId;
                    var migrated = new RelayEnvelope
                    {
                        MatchID = matchId,
                        SenderID = -1,
                        TargetID = -1,
                        Type = RelayMessageType.HostMigrated,
                        Payload = Serialize(match.HostPlayerId)
                    };
                    Broadcast(match, migrated);
                }
            }
        }

        public void HandleJoin(Guid matchId, int playerId, NetPeer peer, int hostPlayerId)
        {
            lock (Lock)
            {
                if (!Matches.TryGetValue(matchId, out var match))
                {
                    match = new ActiveMatch { MatchId = matchId, HostPlayerId = hostPlayerId };
                    Matches[matchId] = match;
                }
                match.Connections.RemoveAll(c => c.PlayerId == playerId);
                match.Connections.Add(new MatchConnection { PlayerId = playerId, Peer = peer });
            }
        }

        public void Handle(RelayEnvelope envelope, NetPeer peer, int lobbyHostPlayerId)
        {
            if (envelope.Type == RelayMessageType.Join)
                HandleJoin(envelope.MatchID, envelope.SenderID, peer, lobbyHostPlayerId);
            else
                Route(envelope);
        }

        public void HandleDisconnectByPeer(NetPeer peer)
        {
            lock (Lock)
            {
                foreach (var match in Matches.Values)
                {
                    var conn = match.Connections.FirstOrDefault(c => c.Peer == peer);
                    if (conn != null)
                    {
                        HandleDisconnect(match.MatchId, conn.PlayerId);
                        return;
                    }
                }
            }
        }

        private void Broadcast(ActiveMatch match, RelayEnvelope envelope)
        {
            var bytes = Serialize(envelope);
            foreach (var conn in match.Connections)
                conn.Peer.Send(bytes, DeliveryMethod.ReliableOrdered);
        }

        private static byte[] Serialize<T>(T obj) => MessagePackSerializer.Serialize(obj);
    }
}
