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

        public NetworkedCommandChannel(Game game, NetSession session)
        {
            Game = game;
            Session = session;
            Session.OnCommandReceived += cmd =>
            {
                if (!Session.IsHost) return;
                var events = Game.ExecuteAndResolve(cmd);
                if(events != null)
                    session.BroadcastEvents(events);
            };
            session.OnEventsReceived += OnEventsFromHost;
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

        public void OnEventsFromHost(List<IGameEvent> events) => Game.ApplyEventLog(events);
    }
}