using BOTB64.Entities;
using BOTB64.Runtime;

namespace BOTB64.Engine
{
    public enum TargetingType
    {
        None = 0,
        Direct = 1,
        BeamNoLos = 2,
        Area = 3,
        Pathfinding = 4,
        BeamLos = 5,
        DirectLos = 6,
        // Direct-clamped center tile plus two flanking tiles one hex to either side
        // (e.g. Akano's Shuriken Toss).
        TripleShot = 7,
        // Walks the beam toward the picked tile up to Radius tiles (not counting the
        // source), stopping early at the first wall tile (e.g. Rassarang's Magma Boulder).
        BeamWall = 8,
        // Two independently picked single-hex targets (e.g. Gravitus's Gravity Tether).
        // Resolved as two sequential Direct picks by the calling UI (SpellCastingAction/
        // GameplayState) - Targeter itself just runs Direct picking twice in a row, so
        // there's no dedicated case for this in UpdateTarget's switch.
        DualDirect = 9
    }

    public class TargetingData
    {
        public TargetingType Type;
        public Hex? Source;
        public int Radius;
        public bool Secret;
        // Area only: size of the AoE disk centered on the (Radius-clamped) picked tile.
        public int AreaRadius;
    }

    public static class Targeter
    {
        public static TargetingData Data { get; set; } = new TargetingData { Type = TargetingType.None, Source = null, Radius = 0 };
        public static Board? Board { get; set; } = null;
        public static List<Tile> Targeted = new();

        private static IEnumerator<List<Tile>>? PathEnumerator;
        private static Hex LastPathFindingDst;

        private static List<List<Tile>> PathCache = new();
        private static int PathIndex = -1;

        public static void SetBoard(Board board)
        {
            Board = board;
        }

        public static void SetTargetingData(TargetingData data)
        {
            Data = data;
        }

        private static void SetHighlightStatus(bool enabled)
        {
            foreach (Tile tile in Targeted)
                tile.Highlighted = enabled;
        }

        public static void Reset()
        {
            // Unhighlight before clearing - once Targeted is empty there's no way back to
            // the tiles that need their Highlighted flag turned back off (matters for a
            // mid-cast reset like the first pick of a DualDirect spell, not just the
            // redundant-but-harmless call already made from UpdateTarget below).
            SetHighlightStatus(false);
            Targeted.Clear();
        }

        public static void UpdateTarget(Hex pickedPoint)
        {
            SetHighlightStatus(false);
            Reset();
            switch (Data.Type)
            {
                case TargetingType.None:
                    break;
                case TargetingType.Direct:
                    TargetDirect(pickedPoint, false);
                    break;
                case TargetingType.BeamNoLos:
                    TargetBeam(pickedPoint, false);
                    break;
                case TargetingType.Pathfinding:
                    TargetPathfinding(pickedPoint);
                    break;
                case TargetingType.Area:
                    TargetArea(pickedPoint);
                    break;
                case TargetingType.BeamLos:
                    TargetBeam(pickedPoint, true);
                    break;
                case TargetingType.DirectLos:
                    TargetDirect(pickedPoint, true);
                    break;
                case TargetingType.TripleShot:
                    TargetTripleShot(pickedPoint);
                    break;
                case TargetingType.BeamWall:
                    TargetBeamWall(pickedPoint);
                    break;
                default:
                    break;
            }

            if(!Data.Secret)
                SetHighlightStatus(true);
        }

        // Clamps to Radius from Source (like TargetDirect) to get the center tile, then
        // walks the ring at that same distance from Source (HexAlgo.Circle) and takes
        // the tiles 2 steps clockwise and counterclockwise from the center along it.
        public static void TargetTripleShot(Hex picked)
        {
            if (Data.Source == null || Board == null)
                return;

            var line = HexAlgo.Beam(Data.Source.Value, picked);
            if (line.Count == 0)
                return;

            int dist = Math.Min(Data.Radius, line.Count - 1);
            Hex center = line[dist];

            void AddIfValid(Hex h)
            {
                Tile? t = Board.GetTile(h);
                if (t != null)
                    Targeted.Add(t);
            }

            AddIfValid(center);

            if (dist <= 0)
                return;

            var ring = HexAlgo.Circle(Data.Source.Value, dist);
            int ringIdx = ring.FindIndex(h => h.Q == center.Q && h.R == center.R);
            if (ringIdx < 0 || ring.Count == 0)
                return;

            AddIfValid(ring[(ringIdx + 2) % ring.Count]);
            AddIfValid(ring[((ringIdx - 2) % ring.Count + ring.Count) % ring.Count]);
        }

        // Walks the beam from Source toward dst, skipping the source tile itself, up to
        // Radius tiles, stopping (without including) the first wall tile it meets.
        public static void TargetBeamWall(Hex dst)
        {
            if (Data.Source == null || Board == null)
                return;

            var line = HexAlgo.Beam(Data.Source.Value, dst);
            for (int i = 1; i <= Data.Radius && i < line.Count; i++)
            {
                Tile? tile = Board.GetTile(line[i]);
                if (tile == null || tile.Type == TileType.Wall)
                    break;
                Targeted.Add(tile);
            }
        }

        public static void TargetDirect(Hex picked, bool lineOfSight)
        {
            if (Data.Source == null || Board == null)
                return;

            var line = HexAlgo.Beam(Data.Source.Value, picked);
            if (line.Count == 0)
                return;

            if (!lineOfSight)
            {
                int idx = Math.Min(Data.Radius, line.Count - 1);
                Targeted.Add(Board.GetTile(line[idx]));
                return;
            }

            Tile? tile = Board.GetTile(line[0]);
            foreach (var h in line)
            {
                if (!Board.IsPassable(h))
                    break;

                var next = Board.GetTile(h);
                if (next == null)
                    break;

                tile = next;
            }

            Targeted.Add(tile);
        }

        // Clamps the pick point to Radius from Source (same as TargetDirect), then fills a
        // disk of AreaRadius around that point - this is both the preview highlight and
        // what the spell script sees as Targets, so no separate AoE query is needed there.
        public static void TargetArea(Hex picked)
        {
            if (Data.Source == null || Board == null)
                return;

            var line = HexAlgo.Beam(Data.Source.Value, picked);
            if (line.Count == 0)
                return;

            int idx = Math.Min(Data.Radius, line.Count - 1);
            Hex center = line[idx];

            for (int dq = -Data.AreaRadius; dq <= Data.AreaRadius; dq++)
            {
                for (int dr = -Data.AreaRadius; dr <= Data.AreaRadius; dr++)
                {
                    var h = new Hex(center.Q + dq, center.R + dr);
                    if (HexAlgo.HexDistance(center, h) <= Data.AreaRadius)
                    {
                        Tile? tile = Board.GetTile(h);
                        if (tile != null)
                            Targeted.Add(tile);
                    }
                }
            }
        }

        public static void TargetBeam(Hex dst, bool lineOfSight)
        {
            if (Data.Source == null || Board == null)
                return;

            var line = HexAlgo.Beam(Data.Source.Value, dst);

            if (!lineOfSight)
            {
                List<Tile> tiles = Board.GetTiles(line);
                for (int i = 0; i <= Data.Radius; i++)
                {
                    if (i >= tiles.Count())
                        break;
                    Targeted.Add(tiles[i]);
                }
                return;
            }

            foreach (var h in line)
            {
                if (!Board.IsPassable(h))
                    break;

                Tile? tile = Board.GetTile(h);
                if (tile == null)
                    break;
                Targeted.Add(tile);
            }
        }

        public static void TargetPathfinding(Hex dst)
        {
            if (Data.Source == null || Board == null)
                return;

            Tile? srcTile = Board.GetTile(Data.Source.Value);
            Tile? dstTile = Board.GetTile(dst);

            if (srcTile == null || dstTile == null)
                return;

            if (PathEnumerator == null || dst != LastPathFindingDst)
            {
                PathEnumerator?.Dispose();
                PathCache.Clear();
                PathIndex = -1;
                LastPathFindingDst = dst;
                PathEnumerator = HexAlgo.YensKShortest(srcTile, dstTile, h => h.IsPassable(), h => h.GetNeighbors(), Data.Radius).GetEnumerator();

                AdvancePathEnumerator();
            }
        }

        public static void GetNextPathfinding()
        {
            if (Data.Type != TargetingType.Pathfinding || PathEnumerator == null)
                return;

            SetHighlightStatus(false);
            AdvancePathEnumerator();
            if (!Data.Secret)
                SetHighlightStatus(true);
        }
        private static void AdvancePathEnumerator()
        {
            if (PathEnumerator == null || Board == null)
                return;

            PathIndex++;
            if (PathIndex >= PathCache.Count)
            {
                if (PathEnumerator.MoveNext())
                    PathCache.Add(PathEnumerator.Current);
                else
                    PathIndex = PathCache.Count > 0 ? 0 : -1;
            }

            if (PathIndex >= 0)
                Targeted = PathCache[PathIndex];
        }

        public static void ResetPathfinding()
        {
            PathEnumerator?.Dispose();
            PathEnumerator = null;
            PathCache.Clear();
            PathIndex = -1;
        }
    }
}
