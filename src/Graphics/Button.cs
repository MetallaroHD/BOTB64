using RB = Raylib_cs.Raylib;
using RL = Raylib_cs;
using System.Drawing;
using BOTB64.Core;

namespace BOTB64.Graphics.UI;

public class Button : UIElement
{
    public RL.Rectangle Bounds;
    public string Text = "";

    public Action? OnClick;

    public override void Update(float dt)
    {
        if(!Visible) return;

        if (InputManager.IsMouseButtonPressed(RL.MouseButton.Left))
            if(RB.CheckCollisionPointRec(InputManager.MousePosition, Bounds))
                OnClick?.Invoke();
    }

    public override void Draw()
    {
        if (!Visible) return;

        RB.DrawRectangleRec(Bounds, RL.Color.LightGray);
        RB.DrawText(Text, (int)Bounds.X + 10, (int)Bounds.Y + 10, 20, RL.Color.Black);
    }
}