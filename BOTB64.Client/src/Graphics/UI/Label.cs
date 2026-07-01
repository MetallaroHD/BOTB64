using System.Numerics;
using RB = Raylib_cs.Raylib;
using RL = Raylib_cs;

namespace BOTB64.Graphics.UI;

public class Label : UIElement
{
    public Vector2 Position;
    public string Text = "";
    public int FontSize = 20;
    public RL.Color Color = RL.Color.Black;

    public override void Update(float dt) { }

    public override void Draw()
    {
        if (!Visible) return;
        RB.DrawText(Text, (int)Position.X, (int)Position.Y, FontSize, Color);
    }
}
