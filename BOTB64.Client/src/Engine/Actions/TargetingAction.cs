using BOTB64.Engine.States;
using BOTB64.Runtime;
using System.Threading.Channels;
using RL = Raylib_cs;

namespace BOTB64.Engine.Actions
{
    public class TargetingAction : ActionBase
    {
        protected Hex MouseAxial;

        public TargetingAction(GameplayState parent) : base(parent)
        {
            Bindings = new KeyBindings();
        }

        public override void Enter()
        {
            UpdateMouseAxial(out bool changed, out bool valid);
            if (valid)
                Targeter.UpdateTarget(MouseAxial);
        }

        public override void Exit()
        {
        }

        public override void Update()
        {
            UpdateMouseAxial(out bool changed, out bool valid);
            if (changed && valid)
                Targeter.UpdateTarget(MouseAxial);
            Bindings.Check();
        }

        public void UpdateMouseAxial(out bool changed, out bool valid)
        {
            valid = false;
            changed = false;
            Hex newMouse = ((GameplayState)Parent).GetMouseAxial(out valid);
            if(!newMouse.Equals(MouseAxial))
                changed = true;
            MouseAxial = newMouse;
        }
    }
}
