using BOTB64.Runtime;
using System.Numerics;
using RB = Raylib_cs.Raylib;
using RL = Raylib_cs;

namespace BOTB64.Graphics.UI
{
    public class IconTextButton : Button
    {
        public RL.Texture2D Icon;
        public string TopRightText = "";

        public int FontSize = 14;
        public int Padding = 6;

        public override void Draw()
        {
            base.Draw();

            // Background
            RB.DrawRectangleRec(Bounds, RL.Color.LightGray);
            DrawHoverOutline();

            // --- ICON CENTERING (same as IconButton) ---
            float availableW = Bounds.Width - Padding * 2;
            float availableH = Bounds.Height - Padding * 2;

            float scale = MathF.Min(
                availableW / Icon.Width,
                availableH / Icon.Height
            );

            float drawW = Icon.Width * scale;
            float drawH = Icon.Height * scale;

            float drawX = Bounds.X + (Bounds.Width - drawW) * 0.5f;
            float drawY = Bounds.Y + (Bounds.Height - drawH) * 0.5f;

            RL.Rectangle source = new RL.Rectangle(0, 0, Icon.Width, Icon.Height);
            RL.Rectangle dest = new RL.Rectangle(drawX, drawY, drawW, drawH);

            RB.DrawTexturePro(
                Icon,
                source,
                dest,
                new Vector2(0, 0),
                0f,
                RL.Color.White
            );

            // --- TOP RIGHT TEXT ---
            if (!string.IsNullOrEmpty(TopRightText))
            {
                int textWidth = RB.MeasureText(TopRightText, FontSize);

                float textX = Bounds.X + Bounds.Width - textWidth - 4;
                float textY = Bounds.Y + 2;

                RB.DrawText(
                    TopRightText,
                    (int)textX,
                    (int)textY,
                    FontSize,
                    RL.Color.Black
                );
            }
        }
    }
}