using BOTB64.Engine.States;
using BOTB64.Entities;
using MessagePack;
using System.Security.Cryptography.X509Certificates;

namespace BOTB64.Engine.Net
{
    public interface IPickChannel
    {
        void Submit(PickCommand command);
    }

    [MessagePackObject]
    public struct PickCommand
    {
        [Key(0)] public int PlayerID;
        [Key(1)] public int PickingIndex;
        [Key(2)] public int CharacterIndex;
    }

    [MessagePackObject]
    public struct PickEvent
    {
        [Key(0)] public int PickingIndex;
        [Key(1)] public int CharacterIndex;
        [Key(2)] public Faction Faction;
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
            Session.OnPickCommandReceived += cmd =>
            {
                if (!Session.IsHost) return;
                if (State.ValidatePick(cmd))
                {
                    var evt = State.ResolvePick(cmd);
                    Session.BroadcastPickEvent(evt);
                }
                else
                {
                    Console.WriteLine("Failed to send pick command!");
                }
            };

            Session.OnPickEventReceived += OnEventFromHost;
        }
        public void Submit(PickCommand command)
        {
            if (Session.IsHost)
            {
                if (State.ValidatePick(command))
                {
                    var evt = State.ResolvePick(command);
                    Session.BroadcastPickEvent(evt);
                }
            }
            else
            {
                Session.SendPickCommand(command);
            }
        }
        public void OnEventFromHost(PickEvent evt)
        {
            State.ApplyPickEvent(evt);
        }
    }
}
