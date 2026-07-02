using BOTB64.Entities;
using MessagePack;
using System.Numerics;

namespace BOTB64.Runtime
{
    [MessagePackObject]
    public readonly struct Hex
    {
        [Key(0)] public int Q { get; }
        [Key(1)] public int R { get; }

        public Hex(int q, int r)
        {
            Q = q;
            R = r;
        }

        public static Hex operator +(Hex h1, Hex h2) => new Hex(h1.Q + h2.Q, h1.R + h2.R);
        public static bool operator ==(Hex a, Hex b) => a.Q == b.Q && a.R == b.R;
        public static bool operator !=(Hex a, Hex b) => !(a == b);
        public override bool Equals(object? obj) => obj is Hex h && this == h;
        public override int GetHashCode() => HashCode.Combine(Q, R);
    }

    /* Hex grid algorithm class, based on https://www.redblobgames.com/grids/hexagons/#line-drawing */
    public static class HexAlgo
    {
        public static readonly float Apothem = 0.5f;
        public static readonly float HexSize = 0.57735f; // derived from apothem = 0.5
        public static readonly (int q, int r)[] Directions = { (1, 0), (1, -1), (0, -1), (-1, 0), (-1, 1), (0, 1) };
        private static readonly int MaxCircleRadius = 50;

        /* The 6 corner offsets relative to the center */
        public static Vector3[] BuildHexOffsets()
        {
            Vector3[] offsets = new Vector3[6];

            for (int i = 0; i < 6; i++)
            {
                float angle = Transform3D.PIO180 * (60 * i);

                offsets[i] = new Vector3(HexSize * MathF.Cos(angle), 0f, HexSize * MathF.Sin(angle));
            }

            return offsets;
        }

        public static Vector3 HexToWorld(Hex h)
        {
            float x = HexSize * 1.5f * h.Q;
            float z = HexSize * MathF.Sqrt(3) * (h.R + h.Q * 0.5f);

            return new Vector3(x, 0f, z);
        }

        public static Hex WorldToHex(Vector3 p)
        {
            float q = (2f / 3f * p.X) / HexSize;
            float r = (-1f / 3f * p.X + MathF.Sqrt(3f) / 3f * p.Z) / HexSize;

            return HexRound(q, r);
        }

        public static (int row, int col) HexToIndex(Hex h, int rowCount, int colCount)
        {
            int rOffset = rowCount / 2;
            int qOffset = colCount / 2;

            return (h.R + rOffset, h.Q + qOffset);
        }

        public static Hex HexSubtract(Hex h1, Hex h2)
        {
            return new Hex(h2.Q - h1.Q, h2.R - h1.R);
        }

        public static int HexDistance(Hex h1, Hex h2)
        {
            Hex h3 = HexSubtract(h1, h2);
            return (int)((MathF.Abs(h3.Q) + MathF.Abs(h3.R) + MathF.Abs(h3.Q + h3.R)) / 2);
        }

        public static List<Hex> Beam(Hex src, Hex dst)
        {
            int dist = HexDistance(src, dst);
            List<Hex> result = new List<Hex>();

            if (dist == 0)
            {
                result.Add(new Hex(src.Q, src.R));
                return result;
            }

            for (int i = 0; i <= dist; i++)
            {
                float t = (float)i / dist;
                var h = HexLerp(src.Q, src.R, dst.Q, dst.R, t);
                var rounded = HexRound(h.q, h.r);
                result.Add(rounded);
            }

            return result;
        }

        public static List<Hex> Circle(Hex center, int rad)
        {
            int radius = Math.Min(rad, MaxCircleRadius); //clamp the radius as to avoid lag

            List<Hex> result = new();

            if (radius == 0)
            {
                result.Add(new Hex(center.Q, center.R));
                return result;
            }

            int q = center.Q + Directions[4].q * radius;
            int r = center.R + Directions[4].r * radius;

            for (int side = 0; side < 6; side++)
            {
                for (int step = 0; step < radius; step++)
                {
                    result.Add(new Hex(q, r));

                    q += Directions[side].q;
                    r += Directions[side].r;
                }
            }

            return result;
        }

        private static (float q, float r) HexLerp(float q1, float r1, float q2, float r2, float t)
        {
            return (q1 + (q2 - q1) * t, r1 + (r2 - r1) * t);
        }

        private static Hex HexRound(float qf, float rf)
        {
            float x = qf;
            float z = rf;
            float y = -x - z;

            int rx = (int)MathF.Round(x);
            int ry = (int)MathF.Round(y);
            int rz = (int)MathF.Round(z);

            float dx = MathF.Abs(rx - x);
            float dy = MathF.Abs(ry - y);
            float dz = MathF.Abs(rz - z);

            if (dx > dy && dx > dz)
                rx = -ry - rz;
            else if (dy > dz)
                ry = -rx - rz;
            else
                rz = -rx - ry;

            return new Hex(rx, rz);
        }

        public static List<Tile>? AStar(Tile start, Tile goal, Func<Tile, bool> isPassable, Func<Tile, IEnumerable<Tile>> neighbors, int maxDepth = int.MaxValue)
        {
            var openSet = new PriorityQueue<Tile, int>();
            var cameFrom = new Dictionary<Tile, Tile>();
            var gScore = new Dictionary<Tile, int> { [start] = 0 };

            openSet.Enqueue(start, HexDistance(start.AxialPosition, goal.AxialPosition));

            while (openSet.Count > 0)
            {
                var current = openSet.Dequeue();

                if (current == goal)
                    return ReconstructPath(cameFrom, current);

                // Don't expand nodes beyond the movement budget
                if (gScore[current] >= maxDepth)
                    continue;

                foreach (var neighbor in neighbors(current))
                {
                    if (!isPassable(neighbor))
                        continue;

                    int tentativeG = gScore.GetValueOrDefault(current, int.MaxValue) + 1;

                    if (tentativeG < gScore.GetValueOrDefault(neighbor, int.MaxValue))
                    {
                        cameFrom[neighbor] = current;
                        gScore[neighbor] = tentativeG;
                        openSet.Enqueue(neighbor, tentativeG + HexDistance(neighbor.AxialPosition, goal.AxialPosition));
                    }
                }
            }

            // Goal unreachable within maxDepth — find the closest reachable tile to goal
            return FindFurthestToward(cameFrom, gScore, goal, maxDepth);
        }

        private static List<Tile> ReconstructPath(Dictionary<Tile, Tile> cameFrom, Tile current)
        {
            var path = new List<Tile> { current };
            while (cameFrom.TryGetValue(current, out var prev))
            {
                current = prev;
                path.Insert(0, current);
            }
            return path;
        }

        public static IEnumerable<List<Tile>> YensKShortest(Tile start, Tile goal, Func<Tile, bool> isPassable, Func<Tile, IEnumerable<Tile>> neighbors, int maxDepth = int.MaxValue)
        {
            List<Tile>? first = AStar(start, goal, isPassable, neighbors, maxDepth);
            if (first == null || first.Count == 0)
                yield break;

            var candidates = new PriorityQueue<List<Tile>, int>();
            var confirmed = new List<List<Tile>> { first };

            yield return first;

            for (int k = 1; ; k++)
            {
                var prevPath = confirmed[k - 1];

                for (int i = 0; i < prevPath.Count - 1; i++)
                {
                    Tile spurNode = prevPath[i];
                    var rootPath = prevPath.Take(i + 1).ToList();

                    var suppressedEdges = new HashSet<(Tile, Tile)>();
                    foreach (var p in confirmed)
                        if (p.Count > i && rootPath.SequenceEqual(p.Take(i + 1)))
                            suppressedEdges.Add((p[i], p[i + 1]));

                    var suppressedNodes = new HashSet<Tile>(rootPath.Take(rootPath.Count - 1));

                    // Remaining budget for the spur portion
                    int spurMaxDepth = maxDepth - (rootPath.Count - 1);

                    List<Tile>? spurPath = AStar(
                        spurNode,
                        goal,
                        t => isPassable(t) && !suppressedNodes.Contains(t),
                        t => neighbors(t).Where(n => !suppressedEdges.Contains((t, n))),
                        spurMaxDepth  // <-- scoped budget
                    );

                    if (spurPath != null && spurPath.Count > 0)
                    {
                        var candidate = rootPath.Concat(spurPath.Skip(1)).ToList();
                        if (!candidates.UnorderedItems.Any(x => x.Element.SequenceEqual(candidate)))
                            candidates.Enqueue(candidate, candidate.Count);
                    }
                }

                if (candidates.Count == 0)
                    yield break;

                var next = candidates.Dequeue();
                confirmed.Add(next);
                yield return next;
            }
        }

        private static List<Tile> FindFurthestToward(Dictionary<Tile, Tile> cameFrom, Dictionary<Tile, int> gScore, Tile goal, int maxDepth)
        {
            // Among all reachable tiles within budget, pick the one closest to goal
            var best = gScore
                .Where(kv => kv.Value <= maxDepth)
                .OrderBy(kv => HexDistance(kv.Key.AxialPosition, goal.AxialPosition))
                .ThenByDescending(kv => kv.Value) // prefer tiles that used more movement
                .FirstOrDefault();

            if (best.Key == null)
                return new List<Tile>();

            return ReconstructPath(cameFrom, best.Key);
        }
    }
}
