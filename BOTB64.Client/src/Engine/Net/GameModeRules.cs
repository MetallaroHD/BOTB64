using BOTB64.Entities;
using BOTB64.Shared;

namespace BOTB64.Engine.Net
{
    public static class GameModeRules
    {
        public static readonly List<Faction> DefaultFactionOrder = new()
        {
            Faction.BlueTeam, Faction.RedTeam, Faction.RedTeam, Faction.BlueTeam,
            Faction.BlueTeam, Faction.RedTeam, Faction.RedTeam, Faction.BlueTeam,
            Faction.BlueTeam, Faction.RedTeam,
        };

        public static int TotalCharacters(GameSizeType sizeType) => sizeType switch
        {
            GameSizeType.v2P or GameSizeType.v2T => 4,
            GameSizeType.v3P or GameSizeType.v3T => 6,
            GameSizeType.v5P or GameSizeType.v5T => 10,
            _ => throw new ArgumentOutOfRangeException(nameof(sizeType))
        };
    }
}