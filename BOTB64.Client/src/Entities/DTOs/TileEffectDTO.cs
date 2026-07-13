using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOTB64.Entities.DTOs
{
    [MessagePackObject]
    public class TileEffectDTO
    {
        [Key(0)] public int ID { get; set; }
        [Key(1)] public string Name { get; set; }
        [Key(2)] public string ScriptURI { get; set; }
        [Key(3)] public string ImageURI { get; set; }
        [Key(4)] public string AnimationURI { get; set; }
        [Key(5)] public string ModelURI { get; set; }
    }
}
