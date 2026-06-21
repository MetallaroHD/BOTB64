using System.Numerics;
using RL = Raylib_cs;

namespace BOTB64.Entities
{
    public enum TileType
    {
        Empty = 0,
        Floor = 1,
        Wall = 2,
        BlueBase = 3,
        RedBase = 4,
    }

    public struct Tile
    {
        public int Q;
        public int R;

        public Vector3 WorldPosition;

        public RL.Color Color = RL.Color.White;
        public TileType Type;

        public Tile(int q, int r, TileType type)
        {
            Q = q;
            R = r;
            Type = type;
        }

        public void SetColor (RL.Color col)
        {
            Color = col;
        }
    }
}
