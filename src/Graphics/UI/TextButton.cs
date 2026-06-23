using RB = Raylib_cs.Raylib;
using RL = Raylib_cs;
using BOTB64.Runtime;

namespace BOTB64.Graphics.UI;

public class TextButton : Button
{
    public string Text = "";

    public override void Update(float dt)
    {
        base.Update(dt);
    }

    public override void Draw()
    {
        base.Draw();

        RB.DrawRectangleRec(Bounds, RL.Color.LightGray);
        DrawHoverOutline();
        RB.DrawText(Text, (int)Bounds.X + 10, (int)Bounds.Y + 10, 20, RL.Color.Black);
    }
}