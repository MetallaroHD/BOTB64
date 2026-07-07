using RL = Raylib_cs;
using RB = Raylib_cs.Raylib;
using BOTB64.Runtime;

namespace BOTB64.Graphics.UI
{
    public class TextBox : IUIElement
    {
        public bool Visible { get; set; } = true;
        public RL.Rectangle Bounds;
        public string Text = "";
        public string Placeholder = "";
        public int MaxLength = 32;
        public bool NumericOnly = false;

        private bool _focused;
        private static RL.Color BorderColor => RL.Color.DarkGray;
        private static RL.Color FocusedBorderColor => RL.Color.Gold;
        private static RL.Color TextColor => RL.Color.White;
        private static RL.Color PlaceholderColor => RL.Color.Gray;

        public void Update(float dt)
        {
            if (!Visible) return;

            bool hovered = RB.CheckCollisionPointRec(UIRenderer.ScreenToUI(InputManager.MousePosition), Bounds);
            if (InputManager.IsLMP)
            {
                if (hovered)
                {
                    _focused = true;
                    InputManager.UseClick();
                }
                else
                {
                    _focused = false;
                }
            }

            if (!_focused) return;

            int key = RB.GetCharPressed();
            while (key > 0)
            {
                char c = char.ToUpperInvariant((char)key);
                bool allowed = NumericOnly ? char.IsDigit(c) || c == '.' : !char.IsControl(c);

                if (allowed && Text.Length < MaxLength)
                    Text += c;

                key = RB.GetCharPressed();
            }

            if (RB.IsKeyPressed(RL.KeyboardKey.Backspace) && Text.Length > 0)
                Text = Text[..^1];
        }

        public void Draw()
        {
            if (!Visible) return;

            RB.DrawRectangleRec(Bounds, RL.Color.Black);
            RB.DrawRectangleLinesEx(Bounds, 2f, _focused ? FocusedBorderColor : BorderColor);

            string display = Text.Length > 0 ? Text : Placeholder;
            RL.Color color = Text.Length > 0 ? TextColor : PlaceholderColor;
            RB.DrawText(display, (int)Bounds.X + 8, (int)Bounds.Y + (int)(Bounds.Height / 2) - 10, 20, color);
        }
    }
}