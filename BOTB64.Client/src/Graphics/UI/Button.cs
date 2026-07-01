using BOTB64.Runtime;
using RL = Raylib_cs;
using RB = Raylib_cs.Raylib;

namespace BOTB64.Graphics.UI
{
    public abstract class Button :UIElement
    {
        public RL.Rectangle Bounds;

        public Action? OnClick;
        public Action? OnHover;

        protected bool IsHovered;
        protected bool WasHovered;

        protected RL.Color BackgroundColor = RL.Color.LightGray;
        protected RL.Color HoverOutlineColor = RL.Color.Gold;
        protected float HoverOutlineThickness = 4f;

        public override void Update(float dt)
        {
            if (!Visible) return;

            IsHovered = RB.CheckCollisionPointRec(InputManager.MousePosition, Bounds);

            if (!WasHovered && IsHovered)
                OnHover?.Invoke();

            WasHovered = IsHovered;

            if (InputManager.IsLMP && IsHovered)
            {
                OnClick?.Invoke();
                InputManager.UseClick();
            }
        }

        public override void Draw()
        {
            if (!Visible) return;
        }

        protected void DrawHoverOutline()
        {
            if (!IsHovered) return;

            RB.DrawRectangleLinesEx(
                Bounds,
                HoverOutlineThickness,
                HoverOutlineColor);
        }
    }
}
