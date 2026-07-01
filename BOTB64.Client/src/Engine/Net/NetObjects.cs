using BOTB64.Entities;

namespace BOTB64.Engine.Net
{
    public class Player
    {
        public int PlayerID;
        public Faction Faction;
        public List<int> OwnedPickSlots = new();
        public bool IsConnected = true;
    }

    public class NetSession
    {
        public Guid MatchID;
        public int LocalPlayerID;
        public int HostPlayerID;
        public bool IsHost => LocalPlayerID == HostPlayerID;
        public List<Player> Players = new();

        public Player LocalPlayer => Players.First(p => p.PlayerID == LocalPlayerID);
    }
}
