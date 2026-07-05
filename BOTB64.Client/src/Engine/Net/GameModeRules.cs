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
            GameSizeType.V2P or GameSizeType.V2T => 4,
            GameSizeType.V3P or GameSizeType.V3T => 6,
            GameSizeType.V5P or GameSizeType.V5T => 10,
            _ => throw new ArgumentOutOfRangeException(nameof(sizeType))
        };
    }
}