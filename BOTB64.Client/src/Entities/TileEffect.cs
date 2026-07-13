using BOTB64.Runtime;

namespace BOTB64.Entities
{
    public enum TileEffectType
    {
        // Invalid
        None = 0,

        // Covers the ground (max 1)
        Terrain = 1,

        // Physical item (max 1)
        Statue = 2,

        // Does not count as any of the other types (eg hotspot)
        Special = 3,
    }

    [Flags]
    public enum TileEffectFlag
    {
        None = 0,
        
        // Makes the tile unwalkable (eg shadow walls)
        Impassable = 1 << 0,

        // Makes the tile block line of sight (eg windwall)
        BlocksLos = 1 << 1,

        // Cannot be removed by other effects of the same type
        Unbreakable = 1 << 2,
    }

    public class TileEffect : ExecutableBase, IReadable
    {
        public Character? Owner;

        public int ID = 0;
        public string Name = "";
        public int Duration = 0;
        public int Remaining = 0;

        public DispelType Dispel = DispelType.None;
        public TileEffectFlag Flags = TileEffectFlag.None;
        public TileEffectType Type = TileEffectType.None;

        public List<Parameter> Parameters = new();
    }
}
