using BOTB64.Engine.States;
using BOTB64.Entities;

namespace BOTB64.Engine.Net
{
    public static class SeatAssignment
    {
        public static List<Player> Assign(GameSizeType sizeType, List<int> connectedPlayerIds, List<Faction> factionOrder, int totalCharacters)
        {
            bool isTeamMode = sizeType is GameSizeType.v2T or GameSizeType.v3T or GameSizeType.v5T;

            if (isTeamMode)
            {
                if (connectedPlayerIds.Count != totalCharacters)
                    throw new InvalidOperationException($"Team mode requires exactly {totalCharacters} players, got {connectedPlayerIds.Count}");

                var players = new List<Player>();
                for (int i = 0; i < totalCharacters; i++)
                {
                    players.Add(new Player
                    {
                        PlayerID = connectedPlayerIds[i],
                        Faction = factionOrder[i],
                        OwnedPickSlots = new List<int> { i }
                    });
                }
                return players;
            }
            else // vNP — exactly 2 players, one per faction
            {
                if (connectedPlayerIds.Count != 2)
                    throw new InvalidOperationException($"1v1 mode requires exactly 2 players, got {connectedPlayerIds.Count}");

                var bluePlayer = new Player { PlayerID = connectedPlayerIds[0], Faction = Faction.BlueTeam };
                var redPlayer = new Player { PlayerID = connectedPlayerIds[1], Faction = Faction.RedTeam };

                for (int i = 0; i < totalCharacters; i++)
                {
                    if (factionOrder[i] == Faction.BlueTeam) bluePlayer.OwnedPickSlots.Add(i);
                    else redPlayer.OwnedPickSlots.Add(i);
                }
                return new List<Player> { bluePlayer, redPlayer };
            }
        }
    }
}
