using System.Numerics;
using RL = Raylib_cs;
using RB = Raylib_cs.Raylib;
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

            for (int q = -Radius; q <= Radius; q++)
            {
                for (int r = -Radius; r <= Radius; r++)
                {
                    if (Math.Abs(q + r) > Radius) continue;

                    Tiles.Add(new Tile
                    {
                        Q = q,
                        R = r,
                        Type = TileType.Floor,
                        Color = RL.Color.Gray
                    });
                }
            }

            // Example: bases
            SetTile(0, 0, TileType.BlueBase, RL.Color.Blue);
            SetTile(Radius, -Radius, TileType.RedBase, RL.Color.Red);
        }

        public void SetTile(int q, int r, TileType type, RL.Color color)
        {
            for (int i = 0; i < Tiles.Count; i++)
            {
                if (Tiles[i].Q == q && Tiles[i].R == r)
                {
                    Tile t = Tiles[i];
                    t.Type = type;
                    t.Color = color;
                    Tiles[i] = t;
                    return;
                }
            }
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

        public void Draw()
        {
            foreach (var tile in Tiles)
            {
                Vector3 pos = HexToWorld(tile.Q, tile.R);

                RB.DrawCylinder(
                    pos + new Vector3(0, 0.01f, 0),
                    HexSize,
                    HexSize,
                    0.02f,
                    6,
                    tile.Color
                );
            }
        }
    }
}