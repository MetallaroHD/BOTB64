namespace BOTB64.Engine.States
{
    public enum GameType
    {
        Local = 0,
        RandomRanked = 1,
        IPMultiplayer = 2,
    }
    public enum GameSizeType
    {
        // 2-player
        v2P = 0,
        v3P = 1,
        v5P = 2,

        // 4/6/10 players
        v2T = 3,
        v3T = 4,
        v5T = 5,
    }
}
