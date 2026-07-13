using BOTB64.Graphics.G3D;
using BOTB64.Runtime;
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
        public Hex AxialPosition;
        public TileType Type;
        public ModelInstance WallModel;

        public Vector3 WorldPosition;
        public List<Tile> Neighbors = new();
        public RL.Color DefaultColor = RL.Color.White;

        public Character? Character;
        public bool Highlighted = false;
        public List<TileEffect> Effects = new();

        public int Q => AxialPosition.Q;
        public int R => AxialPosition.R;

        public Tile(Hex h, TileType type)
        {
            AxialPosition = h;
            Type = type;
        }

        public void SetColor(RL.Color col)
        {
            DefaultColor = col;
        }

        public void AddEffect(TileEffect effect)
        {
            Effects.Add(effect);
        }

        public void RemoveEffect(TileEffect effect)
        {
            Effects.Remove(effect);
        }

        public bool IsPassable()
        {
            if(Type == TileType.Wall || Type == TileType.Empty)
                return false;

            if (Character != null)
                return false;

            for (int i = 0; i < Effects.Count(); i++)
            {
                if (Effects[i].Type.HasFlag(TileEffectFlag.Impassable))
                    return false;
            }

            return true;
        }

        public bool AllowsLos()
        {
            if (Type == TileType.Wall || Type == TileType.Empty)
                return false;

            for (int i = 0; i < Effects.Count(); i++)
            {
                if (Effects[i].Type.HasFlag(TileEffectFlag.BlocksLos))
                    return false;
            }

            return true;
        }

        public List<Tile> GetNeighbors()
        {
            return Neighbors;
        }
    }
}
