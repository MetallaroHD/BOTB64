namespace BOTB64.Graphics.UI;

public interface IUIElement
{
    bool Visible { get; set; }
    // Blocks the mouse from clicking through
    bool Blocking { get; set; }

    void Update(float dt);
    void Draw();
}