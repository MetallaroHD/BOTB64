using System.Numerics;
using RL = Raylib_cs;
using BOTB64.Core;

namespace BOTB64.Entities
{
    internal class Board
    {
        public List<List<Tile>> Tiles = new();

        public float HexSize = 0.57735f; // derived from apothem = 0.5
        public float Apothem = 0.5f;

        public Board()
        {
        }

        public void Generate(DataFile file)
        {
            Tiles.Clear();

        }

        public void SetTile(int q, int r, TileType type, RL.Color color)
        {

        }

        public Vector3 HexToWorld(int q, int r)
        {
            float x = HexSize * MathF.Sqrt(3) * (q + r * 0.5f);
            float z = HexSize * 1.5f * r;

            return new Vector3(x, 0, z);
        }

        public (int q, int r) WorldToHex(Vector3 p)
        {
            float q = (MathF.Sqrt(3) / 3 * p.X - 1f / 3 * p.Z) / HexSize;
            float r = (2f / 3 * p.Z) / HexSize;

            int rq = (int)MathF.Round(q);
            int rr = (int)MathF.Round(r);

            return (rq, rr);
        }

    }
}