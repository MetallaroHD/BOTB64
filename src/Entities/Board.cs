using System.Numerics;
using RL = Raylib_cs;
using RB = Raylib_cs.Raylib;
using BOTB64.Graphics.G3D;

namespace BOTB64.Entities
{
    public class Board
    {
        // Loaded from the .gltf file in the same folder as the .b64m
        public ModelInstance Model;

        // Tiles are flat-top hexagons!
        public List<List<Tile>> Tiles = new();

        public float HexSize = 0.57735f; // derived from apothem = 0.5
        public float Apothem = 0.5f;

        public Vector2 Center = new();

        private readonly Vector3[] HexOffsets = new Vector3[6];

        private static readonly RL.Color[] FloorColors =
        {
            new RL.Color(0xC0, 0xC0, 0xC0, 0xFF),
            new RL.Color(0xE0, 0xE0, 0xE0, 0xFF),
            new RL.Color(0xA0, 0xA0, 0xA0, 0xFF),
        };

        private static readonly RL.Color WallColor = new RL.Color(0x00, 0x00, 0x00, 0xFF);

        private static readonly RL.Color BlueBaseColor = new RL.Color(0x9F, 0xD3, 0xFF, 0xFF);

        private static readonly RL.Color RedBaseColor = new RL.Color(0xFF, 0x9F, 0x9F, 0xFF);

        public Board()
        {
        }

        public void Init()
        {
            BuildHexOffsets();
        }

        private void BuildHexOffsets()
        {
            for (int i = 0; i < 6; i++)
            {
                float angle = Transform.PIO180 * (60 * i);

                HexOffsets[i] = new Vector3(HexSize * MathF.Cos(angle), 0f, HexSize * MathF.Sin(angle));
            }
        }

        public Tile CreateTile(int q, int r, TileType type)
        {
            Tile tile = new Tile
            {
                Q = q,
                R = r,
                Type = type,
                WorldPosition = HexToWorld(q, r)
            };

            ApplyDefaultColor(ref tile);

            return tile;
        }

        public void SetTile(int q, int r, TileType type)
        {
            (int row, int col) = HexToIndex(q, r);

            if (row < 0 || row >= Tiles.Count || col < 0 || col >= Tiles[row].Count)
            {
                return;
            }

            Tile tile = Tiles[row][col];

            tile.Type = type;

            ApplyDefaultColor(ref tile);

            Tiles[row][col] = tile;
        }

        public void Draw()
        {
            Model?.Draw();

            Vector3 shift = new Vector3(0, 0.001f, 0);

            for (int row = 0; row < Tiles.Count; row++)
            {
                List<Tile> line = Tiles[row];

                for (int col = 0; col < line.Count; col++)
                {
                    Tile tile = line[col];

                    if (tile.Type == TileType.Empty)
                        continue;

                    DrawHex(tile.WorldPosition + shift, tile.Color);
                }
            }
        }

        private static void ApplyDefaultColor(ref Tile tile)
        {
            switch (tile.Type)
            {
                case TileType.Wall:
                    tile.Color = WallColor;
                    break;

                case TileType.BlueBase:
                    tile.Color = BlueBaseColor;
                    break;

                case TileType.RedBase:
                    tile.Color = RedBaseColor;
                    break;

                case TileType.Floor:
                default:
                    tile.Color = FloorColors[FloorColorIndex(tile.Q, tile.R)];
                    break;
            }
        }

        private static int FloorColorIndex(int q, int r)
        {
            int idx = (q - r) % 3;

            while (idx < 0)
                idx += 3;

            return idx;
        }

        public void DrawHex(Vector3 center, RL.Color color, float height = 0.02f)
        {
            Vector3 top = center with { Y = center.Y + height };

            for (int i = 0; i < 6; i++)
            {
                Vector3 b = HexOffsets[i];
                Vector3 c = HexOffsets[(i + 1) % 6];

                // Top face
                RB.DrawTriangle3D(top, top + c, top + b, color);

                // Side quad (2 triangles)
                RB.DrawTriangle3D(center + b, top + b, top + c, color);
                RB.DrawTriangle3D(center + b, top + c, center + c, color);
            }
        }

        public Vector3 HexToWorld(int q, int r)
        {
            float x = HexSize * 1.5f * q;
            float z = HexSize * MathF.Sqrt(3) * (r + q * 0.5f);

            return new Vector3(x, 0f, z);
        }

        public (int q, int r) WorldToHex(Vector3 p)
        {
            float q = (2f / 3 * p.X) / HexSize;
            float r = (-1f / 3 * p.X + MathF.Sqrt(3) / 3 * p.Z) / HexSize;

            return ((int)MathF.Round(q), (int)MathF.Round(r));
        }

        private (int row, int col) HexToIndex(int q, int r)
        {
            int rOffset = Tiles.Count / 2;
            int qOffset = Tiles.Count > 0 ? Tiles[0].Count / 2 : 0;
            return (r + rOffset, q + qOffset);
        }

        public void LoadModel(string gltfPath)
        {
            ModelAsset asset = AssetManager.GetModel(gltfPath);

            Model = new ModelInstance(asset);

            Model.Transform.Position = new Vector3(-Center.X, 0f, -Center.Y);
        }
    }
}