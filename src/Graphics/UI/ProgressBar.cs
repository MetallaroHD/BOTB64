using System.Numerics;
using RB = Raylib_cs.Raylib;
using RL = Raylib_cs;

namespace BOTB64.Graphics.UI
{
    public class ProgressBar : UIElement
    {
        public RL.Rectangle Bounds;

        // 0.0f - 1.0f
        private float _value = 1f;

        public float Value
        {
            get => _value;
            set => _value = Math.Clamp(value, 0f, 1f);
        }

        public RL.Color BackgroundColor = new RL.Color(40, 40, 40, 255);
        public RL.Color FillColor = RL.Color.Green;

        public int BorderThickness = 0;
        public RL.Color BorderColor = RL.Color.Black;

        public bool DrawBorder = false;

        public ProgressBar() { }

        public ProgressBar(RL.Rectangle bounds)
        {
            Bounds = bounds;
        }

        public void SetPosition(float x, float y)
        {
            Bounds.X = x;
            Bounds.Y = y;
        }

        public void SetSize(float width, float height)
        {
            Bounds.Width = width;
            Bounds.Height = height;
        }

        public override void Draw()
        {
            // Background
            RB.DrawRectangleRec(Bounds, BackgroundColor);

            // Fill (width based on Value)
            float fillWidth = Bounds.Width * Value;

            RL.Rectangle fillRect = new RL.Rectangle(
                Bounds.X,
                Bounds.Y,
                fillWidth,
                Bounds.Height
            );

            RB.DrawRectangleRec(fillRect, FillColor);

            // Optional border
            if (DrawBorder && BorderThickness > 0)
            {
                RB.DrawRectangleLinesEx(Bounds, BorderThickness, BorderColor);
            }
        }
    }
}