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
        // Snapshot before iterating - an element's Update/OnClick can add or remove
        // elements (e.g. FloatingMessageManager.AddMessage), which would otherwise
        // mutate Elements mid-enumeration and throw.
        foreach (var element in Elements.ToArray())
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

    public void RemoveElement(IUIElement element) => Elements.Remove(element);
    
    public virtual bool IsMouseBlocked()
    {
        Vector2 uiMouse = UIRenderer.ScreenToUI(InputManager.MousePosition);

        foreach (var element in Elements)
        {
            if (element is Button && element.Visible)
                if (RB.CheckCollisionPointRec(uiMouse, ((Button)(element)).Bounds))
                    return true;
        }
        return false;
    }
}