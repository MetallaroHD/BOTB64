using BOTB64.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOTB64.Server.Lobbies
{
    public class Lobby
    {
        public Guid LobbyId { get; init; } = Guid.NewGuid();
        public required GameSizeType GameSizeType { get; init; }
        public string? JoinCode { get; init; } // null for random-matchmaking lobbies, set for custom
        public List<LobbyPlayer> Players { get; } = new();

        public bool IsFull => Players.Count >= GameSizeRules.RequiredPlayerCount(GameSizeType);
    }
}
