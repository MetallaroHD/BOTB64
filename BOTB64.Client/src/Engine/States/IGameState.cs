namespace BOTB64.Engine.States
{
    public interface IGameState
    {
        void OnEnter();
        void OnExit();
        void Update(float deltaTime);
        void Render();
    }
}
