
using BOTB64.Engine.States;
using BOTB64.Entities;
using BOTB64.Runtime;

namespace BOTB64.Engine.Actions
{
    public class SpellCastingAction : TargetingAction
    {
        Character Caster;

        public SpellCastingAction(GameplayState parent) : base(parent)
        {
        }

        public override void Enter()
        {
            CursorManager.SetCursor("Spell");
        }

        public override void Exit()
        {
            throw new NotImplementedException();
        }

        public override void Update()
        {
            throw new NotImplementedException();
        }
    }
}
