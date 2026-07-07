using BOTB64.Entities;
namespace BOTB64.Engine.Net
{
    public interface IGameCommandChannel
    {
        void Submit(IGameCommand command);
    }

    public class LocalCommandChannel : IGameCommandChannel
    {
        private readonly Game Game;
        public LocalCommandChannel(Game game) => Game = game;
        public void Submit(IGameCommand command) => Game.ExecuteAndResolve(command);
    }

    public class NetworkedCommandChannel : IGameCommandChannel
    {
        private readonly Game Game;
        private readonly NetSession Session;
        private readonly Queue<int> PendingDisconnects = new();

        public NetworkedCommandChannel(Game game, NetSession session)
        {
            Game = game;
            Session = session;

            Session.OnCommandReceived += cmd => { /* unchanged */ };
            Session.OnEventsReceived += OnEventsFromHost;

            Session.OnPlayerDisconnected += playerId =>
            {
                if (Session.IsHost) HandleDisconnectReassignment(playerId);
                else PendingDisconnects.Enqueue(playerId);
            };

            Session.OnHostMigrated += newHostId =>
            {
                if (Session.IsHost)
                    while (PendingDisconnects.TryDequeue(out var id))
                        HandleDisconnectReassignment(id);
            };
        }

        public void Submit(IGameCommand command)
        {
            if (Session.IsHost)
            {
                var events = Game.ExecuteAndResolve(command);
                if (events != null)
                    Session.BroadcastEvents(events);
            }
            else 
            {
                Session.SendCommand(command);
            }
        }

        private List<IGameEvent> ReassignCharactersForDisconnectedPlayer(int disconnectedPlayerId)
        {
            var events = new List<IGameEvent>();
            var orphaned = Game.GetCharactersOwnedBy(disconnectedPlayerId);
            if (orphaned.Count == 0) return events;

            foreach (var character in orphaned)
            {
                var newOwner = Session.Players
                    .Where(p => p.IsConnected && p.PlayerID != disconnectedPlayerId && p.Faction == character.Faction)
                    .OrderBy(p => p.PlayerID) // deterministic — same tie-break rule as relay host election
                    .FirstOrDefault();

                if (newOwner == null) continue; // no one left on this team — handled by CheckForGameOverByElimination

                var evt = new CharacterReassignedEvent { CharacterGameID = character.GameID, NewOwnerID = newOwner.PlayerID };
                Game.RecordAndApplyExternal(evt); // see note below — host applies its own copy too, same as other events
                events.Add(evt);
            }
            return events;
        }

        private void HandleDisconnectReassignment(int disconnectedPlayerId)
        {
            var events = ReassignCharactersForDisconnectedPlayer(disconnectedPlayerId);
            if (events.Count > 0)
                Session.BroadcastEvents(events);
            CheckForGameOverByElimination();
        }

        private void CheckForGameOverByElimination()
        {
            if (!Session.IsHost) return;

            var blueHasConnectedOwner = Game.GetCharactersOwnedBy(-999).Any(); // placeholder — see below
            bool blueAlive = Session.Players.Any(p => p.IsConnected && p.Faction == Faction.BlueTeam);
            bool redAlive = Session.Players.Any(p => p.IsConnected && p.Faction == Faction.RedTeam);

            if (!blueAlive || !redAlive)
            {
                var winner = blueAlive ? Faction.BlueTeam : Faction.RedTeam;
                var evt = new TeamEliminatedEvent { Winner = winner };
                Game.RecordAndApplyExternal(evt);
                Session.BroadcastEvents(new List<IGameEvent> { evt });
            }
        }

        public void OnEventsFromHost(List<IGameEvent> events) => Game.ApplyEventLog(events);
    }
}