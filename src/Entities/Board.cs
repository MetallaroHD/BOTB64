using System.Numerics;
using RL = Raylib_cs;
using RB = Raylib_cs.Raylib;
using BOTB64.Graphics.G3D;
using BOTB64.Runtime;
using System.Data;

namespace BOTB64.Entities
{
    public enum SpawnType
    {
        Blue = 0,
        Red = 1,
        Neutral = 2
    }

    public struct SpawnPoint
    {
        public SpawnType Type;
        public Hex Position;
    }

    public class Board
    {
        public ModelInstance Model;
        
        public List<List<Tile>> Tiles = new();
        public Vector2 Center = new();

        public List<SpawnPoint> BlueSpawns { get; set; } = new();
        public List<SpawnPoint> RedSpawns { get; set; } = new();

        public int TileCountRow = 0;
        public int TileCountCol = 0;

        private Vector3[] HexOffsets = new Vector3[6];

        private static readonly RL.Color[] FloorColors =
        {
            new RL.Color(0xC0, 0xC0, 0xC0, 0xFF),
            new RL.Color(0xE0, 0xE0, 0xE0, 0xFF),
            new RL.Color(0xA0, 0xA0, 0xA0, 0xFF),
        };

        private static readonly RL.Color[] BlueBaseColors =
        {
            new RL.Color(0x64, 0x8F, 0xFF, 0xFF),
            new RL.Color(0x84, 0xAF, 0xFF, 0xFF),
            new RL.Color(0xA4, 0xCF, 0xFF, 0xFF),
        };

        private static readonly RL.Color[] RedBaseColors =
        {
            new RL.Color(0xFF, 0x61, 0x00, 0xFF),
            new RL.Color(0xFF, 0x81, 0x20, 0xFF),
            new RL.Color(0xFF, 0xA1, 0x40, 0xFF),
        };

        private static readonly RL.Color WallColor = new RL.Color(0x00, 0x00, 0x00, 0xFF);

        public Board()
        {
        }

        public Tile? GetTile(Hex h)
        {
            (int x, int y) = HexToIndex(h);
            if (IsValidIndex(x, y))
                return Tiles[x][y];
            return null;
        }

        public List<Tile> GetTiles(List<Hex> hexes)
        {
            List<Tile> result = new(hexes.Count());
            foreach (Hex h in hexes)
            {
                Tile? t = GetTile(h);
                if (t != null)
                    result.Add(t);
            }
            return result;
        }

        public bool IsPassable(Hex h)
        {
            Tile? t = GetTile(h);
            if (t == null)
                return false;
            return t.IsPassable();
        }

        public void Init()
        {
            HexOffsets = HexAlgo.BuildHexOffsets();
            TileCountRow = Tiles.Count();
            TileCountCol = Tiles[0].Count();
            BakeNeighbors();
        }

        public (int row, int col) HexToIndex(Hex h) => HexAlgo.HexToIndex(h, TileCountRow, TileCountCol);

        public Tile CreateTile(Hex h, TileType type)
        {
            Tile tile = new Tile(h, type);
            tile.WorldPosition = HexAlgo.HexToWorld(h);
            ApplyDefaultColor(ref tile);

            return tile;
        }

        public void SetTile(Hex h, TileType type)
        {
            (int row, int col) = HexAlgo.HexToIndex(h, TileCountRow, TileCountCol);

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

                    if (!tile.Highlighted)
                        DrawHex(tile.WorldPosition + shift, tile.DefaultColor);
                    else
                        DrawHex(tile.WorldPosition + shift, RL.Color.Yellow);

                    if (tile.Type == TileType.Wall)
                        tile.WallModel.Draw();
                }
            }
        }

        public void RestoreColor(int row, int col)
        {
            Tile tile = Tiles[row][col];

            ApplyDefaultColor(ref tile);
        }

        private static void ApplyDefaultColor(ref Tile tile)
        {
            switch (tile.Type)
            {
                case TileType.Wall:
                    tile.DefaultColor = WallColor;
                    break;

                case TileType.BlueBase:
                    tile.DefaultColor = BlueBaseColors[FloorColorIndex(tile.Q, tile.R)];
                    break;

                case TileType.RedBase:
                    tile.DefaultColor = RedBaseColors[FloorColorIndex(tile.Q, tile.R)];
                    break;

                case TileType.Floor:
                default:
                    tile.DefaultColor = FloorColors[FloorColorIndex(tile.Q, tile.R)];
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

        public bool IsValidIndex(int row, int col)
        {
            return (row >= 0 && row < TileCountRow) && (col >= 0 && col < TileCountCol);
        }

        public bool IsValidHex(Hex h)
        {
            (int r, int c) = HexToIndex(h);
            return IsValidIndex(r, c);
        }

        public void LoadModel(string gltfPath, string wallPath)
        {
            ModelAsset asset = AssetManager.GetModel(gltfPath);

            Model = new ModelInstance(asset);

            Model.Transform.Position = new Vector3(-Center.X, 0f, -Center.Y);

            if (wallPath == "")
                return;

            ModelAsset wall = AssetManager.GetModel(wallPath);
            foreach (var row in Tiles)
            {
                foreach (var tile in row)
                {
                    tile.WallModel = new ModelInstance(wall);
                    tile.WallModel.Transform.Position = tile.WorldPosition + new Vector3(0,0, 0.02f);
                }
            }
        }

        public void MoveCharacter(Character character, List<Tile> path)
        {
            if (path.Count == 0)
                return;

            Tile? oldTile = GetTile(character.Position);
            if (oldTile == null)
                return;

            oldTile.Character = null;
            path.Last().Character = character;
            character.Position = new Hex(path.Last().Q, path.Last().R);
        }

        public void SpawnCharacter(Character character, Hex tile)
        {
            var t = GetTile(tile);

            if (t != null && t.Character != null)
                return;
            t.Character = character;
            character.Position = tile;
            character.Alive = true;
        }

        public void BakeNeighbors()
        {
            // temporary
            var lookup = new Dictionary<Hex, Tile>();
            foreach (var row in Tiles)
                foreach (var tile in row)
                    lookup[tile.AxialPosition] = tile;

            foreach (var row in Tiles)
                foreach (var tile in row)
                    foreach (var dir in HexAlgo.Directions)
                        if (lookup.TryGetValue(tile.AxialPosition + new Hex(dir.q, dir.r), out var neighbor))
                            tile.Neighbors.Add(neighbor);
        }

        public List<Tile> GetNeighbors(Tile tile)
        {
            return tile.Neighbors;
        }

        public List<Tile> GetNeighbors(Hex pos)
        {
            Tile? t = GetTile(pos);
            if (t == null)
                return new List<Tile>();
            return t.Neighbors;
        }

        public IEnumerable<Hex> GetNeighborHexes(Hex h)
        {
            return GetTile(h)?.Neighbors.Select(t => t.AxialPosition) ?? Enumerable.Empty<Hex>();
        }
    }
}