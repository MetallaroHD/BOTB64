using BOTB64.Runtime;
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
        public int Stacks = 0;      // shown on the icon, only if > 1
        public RL.Color BackgroundColor = new RL.Color(50, 50, 50, 255);
        public RL.Color BorderColor = RL.Color.Black;
        public int BorderThickness = 1;
        public int StackFontSize = 12;
        public RL.Color StackColor = RL.Color.White;

        private readonly TooltipBox Tooltip = new();
        private bool HasTooltip = false;

        public void SetIcon(RL.Texture2D texture)
        {
            Icon = texture;
            HasIcon = true;
        }

        public void SetInfo(string name, string tooltip, int remaining)
        {
            var lines = new List<string>();
            if (!string.IsNullOrEmpty(name)) lines.Add(name);
            if (!string.IsNullOrEmpty(tooltip)) lines.Add(tooltip);
            if (remaining > 0) lines.Add($"Remaining: {remaining}");

            HasTooltip = lines.Count > 0;
            Tooltip.SetContent(lines);
        }

        private bool IsHovered() => RB.CheckCollisionPointRec(UIRenderer.ScreenToUI(InputManager.MousePosition), Bounds);

        public override void Draw()
        {
            if (!Visible) return;

            RB.DrawRectangleRec(Bounds, BackgroundColor);

            if (HasIcon)
            {
                float scale = MathF.Min(Bounds.Width / Icon.Width, Bounds.Height / Icon.Height);
                float drawW = Icon.Width * scale;
                float drawH = Icon.Height * scale;
                float drawX = Bounds.X + (Bounds.Width - drawW) * 0.5f;
                float drawY = Bounds.Y + (Bounds.Height - drawH) * 0.5f;
                RB.DrawTexturePro(Icon,
                    new RL.Rectangle(0, 0, Icon.Width, Icon.Height),
                    new RL.Rectangle(drawX, drawY, drawW, drawH),
                    Vector2.Zero, 0f, RL.Color.White);
            }

            if (Stacks > 1)
            {
                string text = Stacks.ToString();
                int textWidth = RB.MeasureText(text, StackFontSize);
                float textX = Bounds.X + Bounds.Width - textWidth - 2;
                float textY = Bounds.Y + Bounds.Height - StackFontSize - 2;
                RB.DrawText(text, (int)textX, (int)textY, StackFontSize, StackColor);
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