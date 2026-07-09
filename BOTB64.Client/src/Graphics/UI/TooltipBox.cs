using System.Numerics;
using RB = Raylib_cs.Raylib;
using RL = Raylib_cs;

namespace BOTB64.Graphics.UI
{
    public class TooltipBox : UIElement
    {
        public string Text = "";
        public int FontSize = 16;
        public int Padding = 6;

        public RL.Color BackgroundColor = new RL.Color(0, 0, 0, 210);
        public RL.Color TextColor = RL.Color.White;

        private RL.Rectangle _bounds;
        public RL.Rectangle Bounds => _bounds;

        public void SetText(string text)
        {
            Text = text ?? "";
            RecalculateSize();
        }

        public void PositionAbove(RL.Rectangle target, float gap = 4f)
        {
            RecalculateSize();

            float x = target.X + (target.Width - _bounds.Width) * 0.5f;
            float y = target.Y - _bounds.Height - gap;

            _bounds.X = x;
            _bounds.Y = y;
        }

        public void SetPosition(Vector2 position)
        {
            RecalculateSize();
            _bounds.X = position.X;
            _bounds.Y = position.Y;
        }

        private void RecalculateSize()
        {
            int textWidth = RB.MeasureText(Text, FontSize);

            _bounds.Width = textWidth + Padding * 2;
            _bounds.Height = FontSize + Padding * 2;
        }

        public override void Draw()
        {
            if (!Visible || string.IsNullOrEmpty(Text)) return;

            RB.DrawRectangleRec(_bounds, BackgroundColor);

            RB.DrawText(
                Text,
                (int)(_bounds.X + Padding),
                (int)(_bounds.Y + Padding),
                FontSize,
                TextColor
            );
        }
    }
}