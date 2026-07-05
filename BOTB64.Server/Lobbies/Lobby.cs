using BOTB64.Shared;

namespace BOTB64.Server.Lobbies
{
    public class Lobby
    {
        public int HostPlayerID { get; set; }

        public Guid LobbyId { get; init; } = Guid.NewGuid();

        public required GameSizeType GameSizeType { get; set; }

        public string? JoinCode { get; init; }

        public List<LobbyPlayer> Players { get; } = new();

        public bool IsFull => Players.Count >= GameSizeRules.RequiredPlayerCount(GameSizeType);

        public bool Started { get; set; } = false;
    }
}