using BOTB64.Graphics.UI;
using BOTB64.Runtime;

namespace BOTB64.Engine.States
{
    public class SettingsState : IGameState
    {
        private readonly SettingsScreen Screen = new();

        public void OnEnter()
        {
            CursorManager.SetCursor("Idle");
            ResourceManager.ClearCache();
            Screen.Controller = this;
            Screen.Enter();
        }

        public void OnExit()
        {
            Screen.Exit();
        }

        public void Update(float dt)
        {
            Screen.Update(dt);
        }

        public void Render()
        {
            Screen.Draw();
        }

        public void MoveToMainMenu()
        {
            StateManager.ChangeState(new MainMenuState());
        }
    }
}