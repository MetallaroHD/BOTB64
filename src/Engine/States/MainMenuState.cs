using BOTB64.Runtime;

namespace BOTB64.Engine.States
{
    public class MainMenuState : IGameState
    {
        public void OnEnter()
        {
        }

        public void OnExit()
        {
            
        }

        public void Update(float dt)
        {
        }

        public void Render()
        {
            Graphics.Graphics.DrawUI(() =>
            {
                RB.DrawText("Press Enter to Start", 500, 350, 20, Color.White);
            });
        }
    }
}