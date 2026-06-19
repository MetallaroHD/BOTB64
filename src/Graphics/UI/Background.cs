using RB = Raylib_cs.Raylib;
using RL = Raylib_cs;

namespace BOTB64.Graphics.UI;

public class Background : UIElement
{
    public RL.Rectangle Bounds;
    public RL.Color Color = RL.Color.DarkGray;

    public override void Update(float dt) { }

    public override void Draw()
    {
        if (!Visible) return;
        RB.DrawRectangleRec(Bounds, Color);
    }
}
