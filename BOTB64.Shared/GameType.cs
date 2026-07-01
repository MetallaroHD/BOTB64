namespace BOTB64.Shared
{
    public enum GameType
    {
        Local = 0,
        RandomRanked = 1,
        IPMultiplayer = 2,
    }

    public enum GameSizeType { v2P, v3P, v5P, v2T, v3T, v5T }

    public static class GameSizeRules
    {
        public static int RequiredPlayerCount(GameSizeType type) => type switch
        {
            GameSizeType.v2P => 2,
            GameSizeType.v3P => 2,
            GameSizeType.v5P => 2,
            GameSizeType.v2T => 4,
            GameSizeType.v3T => 6,
            GameSizeType.v5T => 10,
            _ => throw new ArgumentOutOfRangeException(nameof(type))
        };
    }
}
