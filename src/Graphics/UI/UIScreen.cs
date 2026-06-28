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
        foreach (var element in Elements)
        {
            if (element.Visible)
                element.Draw();
        }
    }

    public void AddElement(IUIElement element) => Elements.Add(element);

    protected void RemoveElement(IUIElement element) => Elements.Remove(element);
    
    public virtual bool IsMouseBlocked()
    {
        foreach (var element in Elements)
        {
            if (element is Button)
                if (RB.CheckCollisionPointRec(InputManager.MousePosition, ((Button)(element)).Bounds))
                    return true;
        }
        return false;
    }
}