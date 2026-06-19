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

    protected void AddElement(IUIElement element) => Elements.Add(element);

    protected void RemoveElement(IUIElement element) => Elements.Remove(element);
}