using BOTB64.Graphics.UI;
using BOTB64.Shared;
using BOTB64.Runtime;

namespace BOTB64.Engine.States
{
    public class MainMenuState : IGameState
    {
        private readonly MainMenuScreen Screen = new();

        private GameType ChosenGameType;
        private GameSizeType ChosenSizeType;

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

        public void SetChosenGT(GameType gameType)
        {
            ChosenGameType = gameType;
        }
        public void SetChosenST(GameSizeType size)
        {
            ChosenSizeType = size;
        }

        public void StartLocalGame() 
        {
            CharacterSelectState cs = new CharacterSelectState();
            cs.GameType = ChosenGameType;
            cs.GameSizeType = ChosenSizeType;

            StateManager.ChangeState(cs);
        }
    }
}