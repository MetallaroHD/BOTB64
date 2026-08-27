
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
            if (Caster == null)
                throw new InvalidOperationException("Caster must be set before entering SpellCastingAction.");
            if (!Caster.ActiveSpells.TryGetValue(SpellBind, out Spell spell))
                throw new InvalidOperationException($"No spell bound to slot {SpellBind}.");

            Hex source = Caster.Position;
            int radius = spell.Range;
            if (spell.TrackedSourceAuraID != 0)
            {
                var game = ((GameplayState)Parent).GetGame();
                int q = (int)EffectProcessor.GetAuraParam(game, Caster.GameID, spell.TrackedSourceAuraID, "Q");
                int r = (int)EffectProcessor.GetAuraParam(game, Caster.GameID, spell.TrackedSourceAuraID, "R");
                source = new Hex(q, r);
                radius = (int)EffectProcessor.GetAuraParam(game, Caster.GameID, spell.TrackedSourceAuraID, "Budget");
            }

            Targeter.SetTargetingData(new TargetingData
            {
                Type = spell.ExplicitTarget,
                Source = source,
                Radius = radius,
            });
            Update();
            base.Enter();
        }

        public override void Update() => base.Update();

        public void SetCurrentCharacter(Character character) => Caster = character;

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
