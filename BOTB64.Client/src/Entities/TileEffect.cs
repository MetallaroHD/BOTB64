using BOTB64.Graphics.Animations;
using BOTB64.Graphics.G3D;
using BOTB64.Runtime;
using RL = Raylib_cs;

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
    public enum TileEffectApplicableTile
    {
        // Do not use
        None = 0,

        // Any floor tile (including bases)
        Floor = 1 << 0,

        // Permanent wall tiles
        Wall = 1 << 1,

        // Only tiles that have an effect making them impassable
        ImpassableTerrain = 1 << 2,
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
        // Usually only assign 1 of the three visuals
        // Terrain tiles may have animated visuals or static pngs 
        public TileVfxAnimation? Animation { get; set; }
        public RL.Texture2D? Texture { get; set; }

        // Statues always use non-animated 3d models (special tiles do what they want)
        public ModelInstance? Model { get; set; }

        // --- Base data (does not change during game) --- //
        public int ID = 0;
        public string Name = "";
        public int Duration = 0;
        public DispelType Dispel = DispelType.None;
        public TileEffectApplicableTile TileType = TileEffectApplicableTile.None;
        public TileEffectFlag Flags = TileEffectFlag.None;
        public TileEffectType Type = TileEffectType.None;

        // --- Volatile data --- //
        public Character? Owner;

        public int Remaining = 0;
        public List<Parameter> Parameters = new();
    }
}
