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
        public ModelInstance EnvModel;
        
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

        public void Draw(Faction? viewerFaction = null)
        {
            Model?.Draw();
            EnvModel?.Draw();

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

                    foreach (var fx in tile.Effects.ToList())
                    {
                        if (fx.Secret && !IsVisibleTo(fx, viewerFaction))
                            continue;

                        if (fx.Texture is RL.Texture2D tex)
                            DrawHexTexture(tile.WorldPosition, tex);

                        fx.Model?.Draw();

                        // VFX intentionally not drawn - no particle/VFX system exists yet.
                    }
                }
            }
        }

        // Secret effects are only visible to the owner's team; local (Session == null) play
        // passes viewerFaction == null, so secret effects never render for anyone.
        private static bool IsVisibleTo(TileEffect fx, Faction? viewerFaction)
        {
            return viewerFaction != null && fx.Owner != null && fx.Owner.Faction == viewerFaction;
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

        // Draws a tile effect's image as a textured overlay on the hex's top face, using the
        // same triangle-fan layout as DrawHex but with UVs so it can carry a texture (raw
        // triangle draws have no UV channel). Sits slightly above the base hex fill to avoid
        // z-fighting.
        public void DrawHexTexture(Vector3 center, RL.Texture2D texture, float height = 0.03f)
        {
            Vector3 top = center with { Y = center.Y + height };

            RL.Rlgl.SetTexture(texture.Id);
            RL.Rlgl.Begin((int)RL.DrawMode.Triangles);
            RL.Rlgl.Color4ub(255, 255, 255, 255);

            for (int i = 0; i < 6; i++)
            {
                Vector3 b = HexOffsets[i];
                Vector3 c = HexOffsets[(i + 1) % 6];

                RL.Rlgl.TexCoord2f(0.5f, 0.5f);
                RL.Rlgl.Vertex3f(top.X, top.Y, top.Z);

                RL.Rlgl.TexCoord2f(HexUvOf(c).X, HexUvOf(c).Y);
                RL.Rlgl.Vertex3f(top.X + c.X, top.Y + c.Y, top.Z + c.Z);

                RL.Rlgl.TexCoord2f(HexUvOf(b).X, HexUvOf(b).Y);
                RL.Rlgl.Vertex3f(top.X + b.X, top.Y + b.Y, top.Z + b.Z);
            }

            RL.Rlgl.End();
            // Without an explicit flush here, this textured draw's texture switch leaves the
            // render batch in a state where every untextured RB.DrawTriangle3D call for the
            // rest of the frame (i.e. every later tile's flat hex fill) silently stops
            // rendering - confirmed by isolated repro; flushing immediately avoids it.
            RL.Rlgl.DrawRenderBatchActive();
            RL.Rlgl.SetTexture(0);
        }

        private static Vector2 HexUvOf(Vector3 offset)
        {
            return new Vector2(0.5f + offset.X / (2f * HexAlgo.HexSize), 0.5f + offset.Z / (2f * HexAlgo.HexSize));
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

        public void LoadModel(string gltfPath, string wallPath, string envPath)
        {
            ModelAsset asset = ResourceManager.GetModel(gltfPath, ModelPurpose.Game);
            ModelAsset env = ResourceManager.GetModel(envPath, ModelPurpose.Game);

            Model = new ModelInstance(asset);
            EnvModel = new ModelInstance(env);

            Model.Transform.Position = new Vector3(-Center.X, 0f, -Center.Y);

            if (wallPath == "")
                return;

            ModelAsset wall = ResourceManager.GetModel(wallPath, ModelPurpose.Game);
            foreach (var row in Tiles)
            {
                foreach (var tile in row)
                {
                    tile.WallModel = new ModelInstance(wall);
                    tile.WallModel.Transform.Position = tile.WorldPosition + new Vector3(0,0, 0.02f);
                }
            }
        }

        public void MoveCharacter(Character character, List<Hex> path)
        {
            if (path.Count == 0)
                return;

            Tile? oldTile = GetTile(character.Position);
            if (oldTile == null)
                return;

            var tiles = GetTiles(path);

            oldTile.Character = null;
            tiles.Last().Character = character;
            character.Position = new Hex(path.Last().Q, path.Last().R);
            if (path.Count >= 2)
            {
                var prev = path[^2];
                var last = path[^1];
                character.Direction = new Hex(last.Q - prev.Q, last.R - prev.R);
            }
        }

        // Like MoveCharacter, but keeps the character's facing (Direction) unchanged -
        // used for forced movement (leaps, knockback, etc) rather than a walked path.
        public void ForceMoveCharacter(Character character, Hex destination)
        {
            Tile? oldTile = GetTile(character.Position);
            Tile? newTile = GetTile(destination);
            if (oldTile == null || newTile == null)
                return;

            oldTile.Character = null;
            newTile.Character = character;
            character.Position = destination;
        }

        public void SpawnCharacter(ref int alloc, Character character, Hex tile, Hex direction)
        {
            var t = GetTile(tile);

            if (t != null && t.Character != null)
                return;
            t.Character = character;
            character.Position = tile;
            character.Direction = direction;
            character.Alive = true;
            character.GameID = alloc;
            alloc++;
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