using BOTB64.Engine.States;
using RL = Raylib_cs;

namespace BOTB64.Engine.Actions
{
    public abstract class ActionBase : IAction
    {
        protected IGameState Parent;
        protected KeyBindings Bindings;

        public ActionBase(IGameState parent) { Parent = parent; }
        public abstract void Enter();
        public abstract void Exit();
        public abstract void Update();
        public void AddBinding(RL.KeyboardKey key, Action action, KeyBindingType type)
        {
            Bindings.Add(key, action, type);
        }
        public void SetLMBinding(Action action)
        {
            Bindings.SetLMAction(action);
        }
        public void SetRMBinding(Action action)
        {
            Bindings.SetRMAction(action);
        }
    }
}
