using MessagePack;

namespace BOTB64.Shared.DTOs
{
    [MessagePackObject]
    public class CharacterDTO
    {
        [Key(0)] public int ID { get; set; }
        [Key(1)] public string Name { get; set; }
        [Key(2)] public string Subdir { get; set; }
        [Key(3)] public string ModelURI { get; set; }
        [Key(4)] public string ScriptURI { get; set; }
        [Key(5)] public string IconURI { get; set; }
        [Key(6)] public bool Enabled { get; set; }
    }
}
