using System.Numerics;
using System.Xml.XPath;

namespace BOTB64.Runtime
{
    /* Hex grid algorithm class, based on https://www.redblobgames.com/grids/hexagons/#line-drawing */
    public static class HexAlgo
    {
        public static readonly float Apothem = 0.5f;
        public static readonly float HexSize = 0.57735f; // derived from apothem = 0.5

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

        public static Vector3 HexToWorld(int q, int r)
        {
            float x = HexSize * 1.5f * q;
            float z = HexSize * MathF.Sqrt(3) * (r + q * 0.5f);

            return new Vector3(x, 0f, z);
        }

        public static (int q, int r) WorldToHex(Vector3 p)
        {
            float q = (2f / 3f * p.X) / HexSize;
            float r = (-1f / 3f * p.X + MathF.Sqrt(3f) / 3f * p.Z) / HexSize;

            return HexRound(q, r);
        }

        public static (int row, int col) HexToIndex(int q, int r, int rowCount, int colCount)
        {
            int rOffset = rowCount / 2;
            int qOffset = colCount / 2;

            return (r + rOffset, q + qOffset);
        }

        public static (int q, int r) HexSubtract(int q1, int r1, int q2, int r2)
        {
            return (q2 - q1, r2 - r1);
        }

        public static int HexDistance(int q1, int r1, int q2, int r2)
        {
            (int q3, int r3) = HexSubtract(q1, r1, q2, r2);
            return (int)((MathF.Abs(q3) + MathF.Abs(r3) + MathF.Abs(q3 + r3)) / 2);
        }

        public static List<(int, int)> Beam(int q1, int r1, int q2, int r2)
        {
            int dist = HexDistance(q1, r1, q2, r2);
            List<(int, int)> result = new List<(int, int)>();

            if (dist == 0)
            {
                result.Add((q1, r1));
                return result;
            }

            for (int i = 0; i <= dist; i++)
            {
                float t = (float)i / dist;
                var h = HexLerp(q1, r1, q2, r2, t);
                var rounded = HexRound(h.q, h.r);
                result.Add(rounded);
            }

            return result;
        }

        private static (float q, float r) HexLerp(float q1, float r1, float q2, float r2, float t)
        {
            return (q1 + (q2 - q1) * t, r1 + (r2 - r1) * t);
        }

        private static (int q, int r) HexRound(float qf, float rf)
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

            return (rx, rz);
        }
    }
}
