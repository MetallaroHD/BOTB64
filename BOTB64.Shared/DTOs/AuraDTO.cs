using MessagePack;

namespace BOTB64.Shared.DTOs
{
    [MessagePackObject]
    public class AuraDTO
    {
        [Key(0)] public int ID { get; set; }
        [Key(1)] public string Name { get; set; }
        [Key(2)] public string ScriptURI { get; set; }
        [Key(3)] public string IconURI { get; set; }
        [Key(4)] public string AnimationURI { get; set; }
    }
}
