namespace BOTB64.Graphics.UI;

public class UIController
{
    private IUIScreen? CurrentScreen;

    public void SetScreen(IUIScreen screen)
    {
        CurrentScreen?.Exit();

        CurrentScreen = screen;

        CurrentScreen.Enter();
    }
}