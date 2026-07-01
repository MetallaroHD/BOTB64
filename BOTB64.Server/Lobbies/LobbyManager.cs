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
        private readonly Dictionary<Guid, Lobby> _lobbies = new();
        private readonly object _lock = new();

        public Lobby CreateCustomLobby(int hostPlayerId, string hostEndpoint, GameSizeType type)
        {
            lock (_lock)
            {
                var lobby = new Lobby { GameSizeType = type, JoinCode = GenerateJoinCode() };
                lobby.Players.Add(new LobbyPlayer { PlayerId = hostPlayerId, PublicEndpoint = hostEndpoint, IsHost = true });
                _lobbies[lobby.LobbyId] = lobby;
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
            lock (_lock)
            {
                var lobby = _lobbies.Values.FirstOrDefault(l => l.JoinCode == joinCode);
                if (lobby == null) return (false, null, "not found");
                if (lobby.IsFull) return (false, null, "full");
                lobby.Players.Add(new LobbyPlayer { PlayerId = playerId, PublicEndpoint = endpoint });
                return (true, lobby, null);
            }
        }

        public Lobby? Get(Guid lobbyId)
        {
            lock (_lock) { return _lobbies.GetValueOrDefault(lobbyId); }
        }

        public void Leave(Guid lobbyId, int playerId)
        {
            lock (_lock)
            {
                if (!_lobbies.TryGetValue(lobbyId, out var lobby)) return;
                var leaving = lobby.Players.FirstOrDefault(p => p.PlayerId == playerId);
                if (leaving == null) return;
                lobby.Players.Remove(leaving);

                if (leaving.IsHost && lobby.Players.Count > 0)
                    lobby.Players.OrderBy(p => p.PlayerId).First().IsHost = true; // deterministic re-election

                if (lobby.Players.Count == 0)
                    _lobbies.Remove(lobbyId);
            }
        }

        private static string GenerateJoinCode() => Guid.NewGuid().ToString("N")[..6].ToUpperInvariant();
    }
}
