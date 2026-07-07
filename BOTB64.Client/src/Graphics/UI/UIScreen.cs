using BOTB64.Runtime;
using RB = Raylib_cs.Raylib;
using System.Numerics;

namespace BOTB64.Graphics.UI;
public abstract class UIScreen : IUIScreen
{
    protected readonly List<IUIElement> Elements = new();

    public virtual void Enter() { }

    public virtual void Exit() { }

    public virtual void Update(float dt)
    {
        foreach (var element in Elements)
        {
            if (element.Visible)
                element.Update(dt);
        }
    }

    public virtual void Draw()
    {
        UIRenderer.Begin();
        foreach (var element in Elements)
        {
            if (element.Visible)
                element.Draw();
        }
        UIRenderer.End();
    }

    public void AddElement(IUIElement element) => Elements.Add(element);

    protected void RemoveElement(IUIElement element) => Elements.Remove(element);
    
    public virtual bool IsMouseBlocked()
    {
        Vector2 uiMouse = UIRenderer.ScreenToUI(InputManager.MousePosition);

        foreach (var element in Elements)
        {
            if (element is Button)
                if (RB.CheckCollisionPointRec(uiMouse, ((Button)(element)).Bounds))
                    return true;
        }
        return false;
    }
}