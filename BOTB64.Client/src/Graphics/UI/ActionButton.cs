using BOTB64.Runtime;
using RL = Raylib_cs;

namespace BOTB64.Graphics.UI
{
    public class ActionButton : IconTextButton
    {
        private readonly TooltipBox Tooltip = new();
        private bool HasTooltip = false;

        public override void Draw()
        {
            base.Draw();

            if (HasTooltip && IsHovered)
            {
                Tooltip.PositionAbove(Bounds);
                Tooltip.Draw();
            }
        }
        public void SetIcon(RL.Texture2D texture)
        {
            Icon = texture;
        }

        public void SetTooltip(List<string> lines)
        {
            HasTooltip = lines.Count > 0;
            Tooltip.SetContent(lines);
        }

        public void Empty()
        {
            Icon = new RL.Texture2D();
            SetTooltip(new());
        }
    }
}
