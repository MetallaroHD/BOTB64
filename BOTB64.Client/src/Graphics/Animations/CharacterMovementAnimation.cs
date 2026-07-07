using BOTB64.Entities;
using BOTB64.Runtime;
using System.Numerics;

namespace BOTB64.Graphics.Animations
{
    public static class Ease
    {
        public static float InOut(float t) => t < 0.5f ? 2f * t * t : 1f - MathF.Pow(-2f * t + 2f, 2f) / 2f;

        public static float In(float t) => t * t;
        public static float Out(float t) => 1f - (1f - t) * (1f - t);
    }

    public class CharacterMoveAnimation : Animation
    {
        private readonly Character Character;
        private readonly List<Vector3> Points;
        private readonly float Duration;
        private float Elapsed;

        private const float LiftHeight = 1f;
        private const float LiftFraction = 0.2f;
        private const float LowerFraction = 0.2f;
        private const float TravelFraction = 1 - LiftFraction - LowerFraction;

        private readonly float[] SegmentEnds;
        private readonly float TotalLength;

        public CharacterMoveAnimation(Character character, List<Hex> path, float speedPerTile = 0.1f)
        {
            Character = character;
            IsBlocking = true;

            if (path.Count <= 0)
                return;

            Points = path.Select(t => HexAlgo.HexToWorld(t)).ToList();
            float[] segLengths = new float[Points.Count - 1];
            TotalLength = 0f;
            for (int i = 0; i < segLengths.Length; i++)
            {
                segLengths[i] = Vector3.Distance(Points[i], Points[i + 1]);
                TotalLength += segLengths[i];
            }

            SegmentEnds = new float[segLengths.Length];
            float cumulative = 0f;
            for (int i = 0; i < segLengths.Length; i++)
            {
                cumulative += segLengths[i];
                SegmentEnds[i] = cumulative / TotalLength; // normalize to [0,1]
            }

            Duration = path.Count * speedPerTile;
        }

        public override void Start()
        {
            Character.IsAnimating = true;
        }

        public override void Update(float dt)
        {
            Elapsed += dt;
            float t = Math.Clamp(Elapsed / Duration, 0f, 1f);

            Character.VisualPosition = SampleArc(t);

            if (t >= 1f)
                IsComplete = true;
        }

        public override void OnComplete()
        {
            Character.IsAnimating = false;
        }

        private Vector3 SampleArc(float t)
        {
            float liftEnd = LiftFraction;
            float travelEnd = LiftFraction + TravelFraction;

            float xzT;    // where along the path we are (0→1)
            float y;

            if (t < liftEnd)
            {
                float phase = t / LiftFraction;
                xzT = 0f;
                y = Ease.InOut(phase) * LiftHeight;
            }
            else if (t < travelEnd)
            {
                float phase = (t - liftEnd) / TravelFraction;
                xzT = Ease.InOut(phase);
                y = LiftHeight;
            }
            else
            {
                float phase = (t - travelEnd) / LowerFraction;
                xzT = 1f;
                y = (1f - Ease.InOut(phase)) * LiftHeight;
            }

            Vector3 xz = SamplePath(xzT);
            return xz with { Y = xz.Y + y };
        }

        // Walk the precomputed segments to find world position at path-t
        private Vector3 SamplePath(float pathT)
        {
            if (pathT <= 0f) return Points[0];
            if (pathT >= 1f) return Points[^1];

            for (int i = 0; i < SegmentEnds.Length; i++)
            {
                if (pathT <= SegmentEnds[i])
                {
                    float segStart = i == 0 ? 0f : SegmentEnds[i - 1];
                    float localT = (pathT - segStart) / (SegmentEnds[i] - segStart);
                    return Vector3.Lerp(Points[i], Points[i + 1], localT);
                }
            }

            return Points[^1];
        }
    }
}
