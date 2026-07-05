namespace BOTB64.Shared
{
    public enum GameType
    {
        Local = 0,
        RandomRanked = 1,
        IPMultiplayer = 2,
    }

    public enum GameSizeType { V2P, V3P, V5P, V2T, V3T, V5T }

    public static class GameSizeRules
    {
        public static int RequiredPlayerCount(GameSizeType type) => type switch
        {
            GameSizeType.V2P => 2,
            GameSizeType.V3P => 2,
            GameSizeType.V5P => 2,
            GameSizeType.V2T => 4,
            GameSizeType.V3T => 6,
            GameSizeType.V5T => 10,
            _ => throw new ArgumentOutOfRangeException(nameof(type))
        };
    }
}
