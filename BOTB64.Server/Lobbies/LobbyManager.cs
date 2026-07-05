using BOTB64.Shared;

namespace BOTB64.Server.Lobbies
{
    public class LobbyManager
    {
        private readonly Dictionary<Guid, Lobby> Lobbies = new();
        private readonly object Lock = new();

        public bool MarkStarted(Guid lobbyId, int playerId)
        {
            lock (Lock)
            {
                if (!Lobbies.TryGetValue(lobbyId, out var lobby)) return false;
                var caller = lobby.Players.FirstOrDefault(p => p.PlayerId == playerId);
                if (caller == null || !caller.IsHost) return false;
                lobby.Started = true;
                return true;
            }
        }
        public Lobby CreateRandomLobby(GameSizeType type, List<(int playerId, string endpoint)> group)
        {
            lock (Lock)
            {
                var lobby = new Lobby
                {
                    GameSizeType = type,
                    JoinCode = null // matchmaking lobbies don't need codes
                };

                foreach (var (playerId, endpoint) in group)
                {
                    lobby.Players.Add(new LobbyPlayer
                    {
                        PlayerId = playerId,
                        DisplayName = $"Player {playerId}", // placeholder until you pass names through matchmaking
                        PublicEndpoint = endpoint,
                        IsHost = lobby.Players.Count == 0
                    });
                }

                if (lobby.Players.Count > 0)
                    lobby.HostPlayerID = lobby.Players[0].PlayerId;

                Lobbies[lobby.LobbyId] = lobby;
                return lobby;
            }
        }

        public Lobby CreateCustomLobby(int hostPlayerId, string hostEndpoint, string hostName, GameSizeType type)
        {
            lock (Lock)
            {
                var lobby = new Lobby
                {
                    GameSizeType = type,
                    JoinCode = GenerateJoinCode()
                };

                lobby.Players.Add(new LobbyPlayer
                {
                    PlayerId = hostPlayerId,
                    DisplayName = hostName,
                    PublicEndpoint = hostEndpoint,
                    IsHost = true
                });

                lobby.HostPlayerID = hostPlayerId;

                Lobbies[lobby.LobbyId] = lobby;
                return lobby;
            }
        }

        public (bool ok, Lobby? lobby, string? error) JoinByCode(string joinCode, int playerId, string endpoint, string name)
        {
            lock (Lock)
            {
                var lobby = Lobbies.Values.FirstOrDefault(l => l.JoinCode == joinCode);
                if (lobby == null) return (false, null, "not found");
                if (lobby.IsFull) return (false, null, "full");

                lobby.Players.Add(new LobbyPlayer
                {
                    PlayerId = playerId,
                    DisplayName = name,
                    PublicEndpoint = endpoint,
                    IsHost = false
                });

                return (true, lobby, null);
            }
        }

        public Lobby? FindByMatchID(Guid lobbyId)
        {
            lock (Lock)
            {
                return Lobbies.GetValueOrDefault(lobbyId);
            }
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
                {
                    var newHost = lobby.Players.OrderBy(p => p.PlayerId).First();
                    newHost.IsHost = true;
                    lobby.HostPlayerID = newHost.PlayerId;
                }

                if (lobby.Players.Count == 0)
                    Lobbies.Remove(lobbyId);
            }
        }

        public bool SetGameSizeType(Guid lobbyId, int playerId, GameSizeType newType)
        {
            lock (Lock)
            {
                if (!Lobbies.TryGetValue(lobbyId, out var lobby)) return false;
                var caller = lobby.Players.FirstOrDefault(p => p.PlayerId == playerId);
                if (caller == null || !caller.IsHost) return false;

                lobby.GameSizeType = newType;
                return true;
            }
        }

        private static string GenerateJoinCode()
            => Guid.NewGuid().ToString("N")[..6].ToUpperInvariant();
    }
}