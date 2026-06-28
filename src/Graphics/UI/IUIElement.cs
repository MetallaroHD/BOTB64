namespace BOTB64.Graphics.UI;

public interface IUIElement
{
    bool Visible { get; set; }

    void Update(float dt);
    void Draw();
}