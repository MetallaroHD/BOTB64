using BOTB64.Graphics;

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
        public Color BaseColor;
        public TileType Type;

        public Tile(int q, int r, Color col, TileType type)
        {
            Q = q;
            R = r;
            BaseColor = col;
            Type = type;
        }
    }
}
