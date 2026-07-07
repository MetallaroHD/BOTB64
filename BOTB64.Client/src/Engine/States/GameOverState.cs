using BOTB64.Engine.Net;
using BOTB64.Entities;
using BOTB64.Graphics.UI;

namespace BOTB64.Engine.States
{
    public class GameOverState : IGameState
    {
        public Faction Winner = Faction.Neutral;

        private GameOverScreen Screen = new();
        public NetSession? Session { get; set; }

        public void OnEnter()
        {
            Screen.SetWinner(Winner);
            Screen.MainMenuButton.OnClick = () =>
            {
                Session?.Disconnect();
                StateManager.ChangeState(new MainMenuState());
            };
            Screen.Enter();
        }

        public void OnExit()
        {
            Screen.Exit();
        }

        public void Update(float deltaTime)
        {
            Screen.Update(deltaTime);
        }

        public void Render()
        {
            Screen.Draw();
        }
    }
}