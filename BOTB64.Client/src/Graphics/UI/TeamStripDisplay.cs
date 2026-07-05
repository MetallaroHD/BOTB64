using RL = Raylib_cs;
using RB = Raylib_cs.Raylib;

namespace BOTB64.Graphics.UI
{
    public class TeamStripDisplay : IUIElement
    {
        public bool Visible { get; set; } = true;
        public RL.Rectangle Bounds;
        public RL.Color BackgroundColor = RL.Color.DarkBlue;
        public int IconSize = 64;
        public int Padding = 8;

        private readonly List<RL.Texture2D> _icons = new();

        public void AddIcon(RL.Texture2D icon) => _icons.Add(icon);

        public void Update(float dt) { }

        public void Draw()
        {
            if (!Visible) return;
            RB.DrawRectangleRec(Bounds, BackgroundColor);

            for (int i = 0; i < _icons.Count; i++)
            {
                float x = Bounds.X + Padding + i * (IconSize + Padding);
                float y = Bounds.Y + (Bounds.Height - IconSize) / 2f;
                if (x + IconSize > Bounds.X + Bounds.Width) break;
                RB.DrawTexturePro(_icons[i],
                    new RL.Rectangle(0, 0, _icons[i].Width, _icons[i].Height),
                    new RL.Rectangle(x, y, IconSize, IconSize),
                    System.Numerics.Vector2.Zero, 0f, RL.Color.White);
            }
        }
    }
}