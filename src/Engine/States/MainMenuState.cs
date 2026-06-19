using BOTB64.Graphics.UI;

namespace BOTB64.Engine.States
{
    public class MainMenuState : IGameState
    {
        private readonly MainMenuScreen Screen = new();

        public void OnEnter()
        {
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
    }
}