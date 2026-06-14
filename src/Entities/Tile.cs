using RL = Raylib_cs;

namespace BOTB64.Entities
{
    enum TileType
    {
        Empty = 0,
        Floor = 1,
        Wall = 2,
        BlueBase = 3,
        RedBase = 4,
    }

    struct Tile
    {
        public int Q;
        public int R;
        public RL.Color Color;
        public TileType Type;
    }
}
