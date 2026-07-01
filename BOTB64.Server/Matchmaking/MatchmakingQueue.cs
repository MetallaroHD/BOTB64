using BOTB64.Server.Lobbies;
using BOTB64.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOTB64.Server.Matchmaking
{
    public class MatchmakingQueue
    {
        private readonly Dictionary<GameSizeType, List<(int playerId, string endpoint)>> _waiting = new();
        private readonly LobbyManager _lobbyManager;
        private readonly object _lock = new();

        public MatchmakingQueue(LobbyManager lobbyManager) => _lobbyManager = lobbyManager;

        public Guid? Enqueue(int playerId, string endpoint, GameSizeType type)
        {
            lock (_lock)
            {
                if (!_waiting.ContainsKey(type)) _waiting[type] = new();
                _waiting[type].Add((playerId, endpoint));

                int required = GameSizeRules.RequiredPlayerCount(type);
                // TODO: swap this FIFO grouping for MMR-based grouping once ranking rules exist
                if (_waiting[type].Count >= required)
                {
                    var group = _waiting[type].Take(required).ToList();
                    _waiting[type].RemoveRange(0, required);

                    var lobby = _lobbyManager.CreateRandomLobby(type, group);
                    return lobby.LobbyId;
                }
                return null; // still waiting
            }
        }
    }
}
