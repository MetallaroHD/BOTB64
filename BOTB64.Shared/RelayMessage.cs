using MessagePack;

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
        MatchStart = 8,
    }

    [MessagePackObject]
    public struct RelayEnvelope
    {
        [Key(0)] public Guid MatchID;
        [Key(1)] public RelayMessageType Type;
        [Key(2)] public int SenderID;
        [Key(3)] public int TargetID; //-1 = broadcast to all
        [Key(4)] public byte[] Payload;
    }
}
