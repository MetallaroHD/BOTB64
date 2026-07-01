using BOTB64.Runtime;
using System.ComponentModel.Design;
using RL = Raylib_cs;

namespace BOTB64.Engine
{
    public enum KeyBindingType
    {
        Press = 0,
        Hold = 1,
    }

    internal class KeyBinding
    {
        internal KeyBindingType Type;
        internal RL.KeyboardKey? Key;
        internal Action? Action;

        public KeyBinding(RL.KeyboardKey key, Action action, KeyBindingType type)
        {
            Key = key;
            Action = action;
            Type = type;
        }

        public void Activate()
        {
            if (Action == null)
                return;
            Action.Invoke();
        }
    }

    public class KeyBindings
    {
        private Action? LMAction;
        private Action? RMAction; // Usually leave empty since right mouse is reserved to camera
        private List<KeyBinding> Bindings = new List<KeyBinding>();

        public void Add(RL.KeyboardKey key, Action action, KeyBindingType type)
        {
            Bindings.Add(new KeyBinding(key, action, type));
        }

        public void Remove(RL.KeyboardKey key)
        {
            foreach (var binding in Bindings)
            {
                if (binding.Key == key)
                    Bindings.Remove(binding);
            }
        }

        public void SetLMAction(Action action)    
        { 
            LMAction = action; 
        }

        public void SetRMAction(Action action)
        {
            RMAction = action;
        }

        public void Clear()
        {
            Bindings.Clear();
        }

        public void Check() 
        {
            if(LMAction != null && InputManager.IsLMP)
                LMAction.Invoke();

            if(RMAction != null && InputManager.IsRMP) 
                RMAction.Invoke();

            foreach (var binding in Bindings)
            {
                if (binding.Key == null)
                    return;
                if (binding.Type == KeyBindingType.Press)
                {
                    if (InputManager.IsKeyPressed(binding.Key.Value))
                        binding.Activate();
                }
                else if (binding.Type == KeyBindingType.Hold)
                {
                    if (InputManager.IsKeyDown(binding.Key.Value))
                        binding.Activate();
                }
            }
        }
    }
}
