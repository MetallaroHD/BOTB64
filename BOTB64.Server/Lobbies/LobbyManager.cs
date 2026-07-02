using BOTB64.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOTB64.Server.Lobbies
{
    public class LobbyManager
    {
        private readonly Dictionary<Guid, Lobby> Lobbies = new();
        private readonly object Lock = new();

        public Lobby CreateCustomLobby(int hostPlayerId, string hostEndpoint, GameSizeType type)
        {
            lock (Lock)
            {
                var lobby = new Lobby { GameSizeType = type, JoinCode = GenerateJoinCode() };
                lobby.Players.Add(new LobbyPlayer { PlayerId = hostPlayerId, PublicEndpoint = hostEndpoint, IsHost = true });
                Lobbies[lobby.LobbyId] = lobby;
                return lobby;
            }
        }

        public Lobby CreateRandomLobby(GameSizeType type, List<(int, string)> group)
        {
            //TBI
            return new Lobby { GameSizeType = type };
        }

        public (bool ok, Lobby? lobby, string? error) JoinByCode(string joinCode, int playerId, string endpoint)
        {
            lock (Lock)
            {
                var lobby = Lobbies.Values.FirstOrDefault(l => l.JoinCode == joinCode);
                if (lobby == null) return (false, null, "not found");
                if (lobby.IsFull) return (false, null, "full");
                lobby.Players.Add(new LobbyPlayer { PlayerId = playerId, PublicEndpoint = endpoint });
                return (true, lobby, null);
            }
        }

        public Lobby? FindByMatchID(Guid lobbyId)
        {
            lock (Lock) { return Lobbies.GetValueOrDefault(lobbyId); }
        }

        public void Leave(Guid lobbyId, int playerId)
        {
            lock (Lock)
            {
                if (!Lobbies.TryGetValue(lobbyId, out var lobby)) return;
                var leaving = lobby.Players.FirstOrDefault(p => p.PlayerId == playerId);
                if (leaving == null) return;
                lobby.Players.Remove(leaving);

                if (leaving.IsHost && lobby.Players.Count > 0)
                    lobby.Players.OrderBy(p => p.PlayerId).First().IsHost = true;

                if (lobby.Players.Count == 0)
                    Lobbies.Remove(lobbyId);
            }
        }

        private static string GenerateJoinCode() => Guid.NewGuid().ToString("N")[..6].ToUpperInvariant();
    }
}
