namespace BOTB64.Graphics.UI
{
    public class ActionButton : IconTextButton
    {
        TooltipBox? Tooltip;

        public override void Draw()
        {
            base.Draw();

            if (Tooltip != null && IsHovered)
            {
                Tooltip.PositionAbove(Bounds);
                Tooltip.Draw();
            }
        }
    }
}
