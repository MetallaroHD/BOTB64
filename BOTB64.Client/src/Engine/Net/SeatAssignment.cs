using BOTB64.Engine.Net;
using BOTB64.Entities;
using BOTB64.Shared;

public static class SeatAssignment
{
    public static List<Player> Assign(GameSizeType sizeType, List<LobbyPlayerDto> connectedPlayers, List<Faction> factionOrder, int totalCharacters)
    {
        bool isTeamMode = sizeType is GameSizeType.V2T or GameSizeType.V3T or GameSizeType.V5T;
        var players = new List<Player>();

        if (isTeamMode)
        {
            for (int i = 0; i < connectedPlayers.Count; i++)
            {
                var src = connectedPlayers[i];
                if (i < totalCharacters)
                    players.Add(new Player { PlayerID = src.PlayerId, DisplayName = src.DisplayName, Faction = factionOrder[i], OwnedPickSlots = new List<int> { i } });
                else
                    players.Add(new Player { PlayerID = src.PlayerId, DisplayName = src.DisplayName, Faction = Faction.Spectator, OwnedPickSlots = new() });
            }
        }
        else // vNP
        {
            for (int i = 0; i < connectedPlayers.Count; i++)
            {
                var src = connectedPlayers[i];
                if (i == 0)
                {
                    var p = new Player { PlayerID = src.PlayerId, DisplayName = src.DisplayName, Faction = Faction.BlueTeam, OwnedPickSlots = new() };
                    for (int slot = 0; slot < totalCharacters; slot++)
                        if (factionOrder[slot] == Faction.BlueTeam) p.OwnedPickSlots.Add(slot);
                    players.Add(p);
                }
                else if (i == 1)
                {
                    var p = new Player { PlayerID = src.PlayerId, DisplayName = src.DisplayName, Faction = Faction.RedTeam, OwnedPickSlots = new() };
                    for (int slot = 0; slot < totalCharacters; slot++)
                        if (factionOrder[slot] == Faction.RedTeam) p.OwnedPickSlots.Add(slot);
                    players.Add(p);
                }
                else
                {
                    players.Add(new Player { PlayerID = src.PlayerId, DisplayName = src.DisplayName, Faction = Faction.Spectator, OwnedPickSlots = new() });
                }
            }
        }
        return players;
    }
}