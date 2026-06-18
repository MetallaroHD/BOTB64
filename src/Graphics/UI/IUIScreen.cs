namespace BOTB64.Graphics.UI;

public interface IUIScreen
{
    void Enter();
    void Exit();

    void Update(float dt);
    void Draw();
}