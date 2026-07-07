using BOTB64.Runtime;
using BOTB64.Entities.Effects;

namespace BOTB64.Entities
{
    [Flags]
    public enum TileEffectType
    {
        None = 0,
        
        // Makes the tile unwalkable (eg shadow walls)
        Impassable = 1 << 0,

        // Makes the tile block line of sight (eg windwall)
        BlocksLos = 1 << 0,
    }

    public class TileEffect : ExecutableBase, IReadable
    {
        public Character? Owner;

        public int ID = 0;
        public string Name = "";
        public int Duration = 0;
        public int Remaining = 0;

        public DispelType Dispel = DispelType.None;
        public TileEffectType Type = TileEffectType.None;

        public List<Parameter> Parameters = new();
    }
}
