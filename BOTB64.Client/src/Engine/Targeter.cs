using BOTB64.Entities;
using BOTB64.Runtime;
using System.Security.Cryptography;

namespace BOTB64.Engine
{
    public enum TargetingType
    {
        None = 0,
        Direct = 1,
        BeamNoLos = 2,
        Area = 3,
        Pathfinding = 4
    }

    public class TargetingData
    {
        public TargetingType Type;
        public Hex? Source;
        public int Radius;
        public bool Secret;
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
                default:
                    break;
            }

            if(!Data.Secret)
                SetHighlightStatus(true);
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
