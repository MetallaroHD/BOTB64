using MessagePack;

namespace BOTB64.Shared.DTOs
{
    [MessagePackObject]
    public class LevelDTO
    {
        [Key(0)] public int ID { get; set; }
        [Key(1)] public string Name { get; set; }
        [Key(2)] public string Subdir { get; set; }
        [Key(3)] public string Script { get; set; }
        [Key(4)] public string Model { get; set; }
        [Key(5)] public string Wall { get; set; }
        [Key(6)] public string Environment { get; set; }
        [Key(7)] public string Shader { get; set; }
    }
}
