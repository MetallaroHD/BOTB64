using System.Numerics;
using RB = Raylib_cs.Raylib;
using RL = Raylib_cs;

namespace BOTB64.Graphics.UI
{
    public class TimedLabel : Label
    {
        public float Duration;
        public float MaxDuration;
        public float StartY = 250f;
        public float EndY = 50f;
        public bool Finished { get; private set; }

        // Base color at full opacity, so we can fade Color.A without losing the original RGB.
        private RL.Color _baseColor;

        private const float ScreenWidth = 1280f;

        public void Setup(string text, float duration, int fontSize, RL.Color color, float startY = 100f, float endY = 45f)
        {
            Text = text;
            FontSize = fontSize;
            Duration = duration;
            MaxDuration = duration;
            StartY = startY;
            EndY = endY;
            _baseColor = color;
            Color = color;
            Finished = false;
            Visible = true;

            // Set initial position/alpha immediately so it doesn't pop in at t=0 on first frame.
            Apply(0f);
        }

        public override void Update(float dt)
        {
            if (Finished) return;

            Duration -= dt;
            if (Duration <= 0f)
            {
                Duration = 0f;
                Finished = true;
                Visible = false;
                return;
            }

            // t goes from 0 (just spawned) to 1 (about to expire)
            float t = 1f - (Duration / MaxDuration);
            Apply(t);
        }

        private void Apply(float t)
        {
            // Move from StartY to EndY over the lifetime.
            float y = StartY + (EndY - StartY) * t;

            // Recompute centering every time in case Text/FontSize changes.
            int textWidth = RB.MeasureText(Text, FontSize);
            float x = (ScreenWidth - textWidth) / 2f;

            Position = new Vector2(x, y);

            // Fade OUT: fully opaque at t=0, transparent at t=1.
            float alpha = 1f - t;
            RL.Color c = _baseColor;
            c.A = (byte)(255 * alpha);
            Color = c;
        }
    }
}