namespace BOTB64.Shared
{
    public enum RelayMessageType : byte
    {
        Join = 0,
        Command = 1,
        EventBatch = 2,
        HostMigrated = 3,
        PlayerDisconnected = 4,
        PlayerReconnected = 5,
        PickCommand = 6,
        PickEvent = 7,
    }

    public struct RelayEnvelope
    {
        public Guid MatchID;
        public RelayMessageType Type;
        public int SenderID;
        public int TargetID; //-1 = broadcast to all
        public byte[] Payload;
    }
}
