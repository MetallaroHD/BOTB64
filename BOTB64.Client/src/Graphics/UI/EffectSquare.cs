using System;
using System.Numerics;
using RB = Raylib_cs.Raylib;
using RL = Raylib_cs;

namespace BOTB64.Graphics.UI
{
    public class EffectSquare : UIElement
    {
        public RL.Rectangle Bounds;

        public RL.Texture2D Icon;
        public bool HasIcon = false;

        public int Duration = 0;
        public bool ShowDuration = true;

        public RL.Color BackgroundColor = new RL.Color(50, 50, 50, 255);
        public RL.Color BorderColor = RL.Color.Black;
        public int BorderThickness = 1;

        public int DurationFontSize = 12;
        public RL.Color DurationColor = RL.Color.White;

        private readonly TooltipBox Tooltip = new();
        private bool HasTooltip = false;

        public void SetIcon(RL.Texture2D texture)
        {
            Icon = texture;
            HasIcon = true;
        }

        public void SetTooltip(string text)
        {
            HasTooltip = !string.IsNullOrEmpty(text);
            Tooltip.SetText(text);
        }

        private bool IsHovered()
        {
            Vector2 mouse = RB.GetMousePosition();
            return RB.CheckCollisionPointRec(mouse, Bounds);
        }

        public override void Draw()
        {
            if (!Visible) return;

            RB.DrawRectangleRec(Bounds, BackgroundColor);

            if (HasIcon)
            {
                float scale = MathF.Min(
                    Bounds.Width / Icon.Width,
                    Bounds.Height / Icon.Height
                );

                float drawW = Icon.Width * scale;
                float drawH = Icon.Height * scale;
                float drawX = Bounds.X + (Bounds.Width - drawW) * 0.5f;
                float drawY = Bounds.Y + (Bounds.Height - drawH) * 0.5f;

                RL.Rectangle source = new RL.Rectangle(0, 0, Icon.Width, Icon.Height);
                RL.Rectangle dest = new RL.Rectangle(drawX, drawY, drawW, drawH);

                RB.DrawTexturePro(Icon, source, dest, new Vector2(0, 0), 0f, RL.Color.White);
            }

            if (ShowDuration && Duration > 0)
            {
                string text = Duration.ToString();
                int textWidth = RB.MeasureText(text, DurationFontSize);

                float textX = Bounds.X + Bounds.Width - textWidth - 2;
                float textY = Bounds.Y + Bounds.Height - DurationFontSize - 2;

                RB.DrawText(text, (int)textX, (int)textY, DurationFontSize, DurationColor);
            }

            if (BorderThickness > 0)
                RB.DrawRectangleLinesEx(Bounds, BorderThickness, BorderColor);

            if (HasTooltip && IsHovered())
            {
                Tooltip.PositionAbove(Bounds);
                Tooltip.Draw();
            }
        }
    }
}