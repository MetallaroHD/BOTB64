using System.Numerics;

namespace BOTB64.Graphics.UI;

public abstract class UIElement : IUIElement
{
    public bool Visible { get; set; } = true;

    public virtual void Update(float dt)
    {
    }

    public abstract void Draw();
}