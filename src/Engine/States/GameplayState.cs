using BOTB64.Runtime;

namespace BOTB64.Engine.States
{
    public class GameplayState : IGameState
    {
        private Game Game = new();   

        public void OnEnter() => Game.Initialize();   
        public void OnExit() => Game.Unload();   

        public void Update(float dt)
        {
            Game.Update(dt);
        }

        public void Render() => Game.Render();
    }
}