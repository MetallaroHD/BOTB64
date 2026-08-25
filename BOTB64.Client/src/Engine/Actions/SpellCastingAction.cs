
using BOTB64.Engine.States;
using BOTB64.Entities;
using BOTB64.Runtime;
using System.Collections.Generic;

namespace BOTB64.Engine.Actions
{
    public class SpellCastingAction : TargetingAction
    {
        Character Caster;
        public int SpellBind;

        public SpellCastingAction(GameplayState parent) : base(parent)
        {
        }

        public override void Enter()
        {
            CursorManager.SetCursor("Spell");
            // set appropriate targeter
            base.Enter();
        }

        public override void Exit()
        {
            throw new NotImplementedException();
        }

        public override void Update()
        {
            throw new NotImplementedException();
        }

        public List<Hex> GetExplicitTarget()
        {
            List<Hex> tg = new();
            foreach (Tile t in Targeter.Targeted)
            {
                tg.Add(t.AxialPosition);
            }
            return tg;
        }
    }
}
