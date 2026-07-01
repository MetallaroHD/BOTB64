using BOTB64.Engine.States;
using BOTB64.Entities;
using System.Security.Cryptography.X509Certificates;

namespace BOTB64.Engine.Net
{
    public interface IPickChannel
    {
        void Submit(PickCommand command);
    }

    public struct PickCommand
    {
        public int PlayerID;
        public int PickingIndex;
        public int CharacterIndex;
    }

    public struct PickEvent
    {
        public int PickingIndex;
        public int CharacterIndex;
        public Faction Faction;
    }

    public class LocalPickChannel : IPickChannel
    {
        private readonly CharacterSelectState State;
        public LocalPickChannel(CharacterSelectState state) => State = state;
        public void Submit(PickCommand command) => State.ResolvePick(command);
    }

    public class NetworkedPickChannel : IPickChannel
    {
        private readonly CharacterSelectState State;
        private readonly NetSession Session;
        public NetworkedPickChannel(CharacterSelectState state, NetSession session)
        {
            State = state;
            Session = session;
        }
        public void Submit(PickCommand command)
        {
            if (Session.IsHost)
            {
                if (State.ValidatePick(command))
                {
                    var evt = State.ResolvePick(command);
                    //Session.Broadcast(evt);
                }
                else
                {
                    //Session.NotifyRejected();
                }
            }
            else 
            {
                //Session.SendToHost(command);
            }
        }
        public void OnEventFromHost(PickEvent evt) => State.ApplyPickEvent(evt);
    }
}
