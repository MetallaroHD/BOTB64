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

    public class Tile
    {
        public int Q;
        public int R;

        public Vector3 WorldPosition;

        public RL.Color Color = RL.Color.White;
        public TileType Type;
        public List<TileEffect> Effects = new();

        public Tile(int q, int r, TileType type)
        {
            Q = q;
            R = r;
            Type = type;
        }

        public void SetColor(RL.Color col)
        {
            Color = col;
        }

        public void AddEffect(TileEffect effect)
        {
            Effects.Add(effect);
        }

        public void RemoveEffect(TileEffect effect)
        {
            Effects.Remove(effect);
        }
    }
}
