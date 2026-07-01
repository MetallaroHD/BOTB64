using BOTB64.Runtime;
using System.Numerics;
using static System.Net.Mime.MediaTypeNames;
using RB = Raylib_cs.Raylib;
using RL = Raylib_cs;

namespace BOTB64.Graphics.UI
{
    public class IconButton : Button
    {
        public RL.Texture2D Icon;

        public override void Update(float dt)
        {
            base.Update(dt);
        }

        public override void Draw()
        {
            base.Draw();

            RB.DrawRectangleRec(Bounds, RL.Color.LightGray);
            DrawHoverOutline();

            float padding = 6f;
            float availableW = Bounds.Width - padding * 2;
            float availableH = Bounds.Height - padding * 2;
            float scale = MathF.Min(availableW / Icon.Width, availableH / Icon.Height);
            float drawW = Icon.Width * scale;
            float drawH = Icon.Height * scale;
            float drawX = Bounds.X + (Bounds.Width - drawW) * 0.5f;
            float drawY = Bounds.Y + (Bounds.Height - drawH) * 0.5f;

            RL.Rectangle source = new RL.Rectangle(0, 0, Icon.Width, Icon.Height);

            RL.Rectangle dest = new RL.Rectangle(drawX, drawY, drawW, drawH);

            RB.DrawTexturePro(Icon, source, dest, new Vector2(0, 0), 0f, RL.Color.White);
        }
    }
}
