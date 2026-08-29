
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

        // DualDirect spells (e.g. Gravitus's Gravity Tether) resolve as two sequential
        // Direct picks - this holds the first pick's result between the two confirm
        // clicks. Null before the first pick is confirmed.
        private List<Hex> FirstPick;

        public SpellCastingAction(GameplayState parent) : base(parent)
        {
        }

        public override void Enter()
        {
            FirstPick = null;
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

            // DualDirect is two Direct picks back to back - the Targeter itself only
            // knows how to run one Direct pick at a time.
            var targetingType = spell.ExplicitTarget == TargetingType.DualDirect ? TargetingType.Direct : spell.ExplicitTarget;

            Targeter.SetTargetingData(new TargetingData
            {
                Type = targetingType,
                Source = source,
                Radius = radius,
                AreaRadius = spell.AreaRadius,
            });
            Update();
            base.Enter();
        }

        public override void Update() => base.Update();

        public void SetCurrentCharacter(Character character) => Caster = character;

        // True while a DualDirect spell is still waiting on its first pick to be confirmed.
        public bool NeedsFirstPick(Spell spell) => spell.ExplicitTarget == TargetingType.DualDirect && FirstPick == null;

        // Stashes the just-confirmed first pick and resets targeting for the second one.
        public void ConfirmFirstPick()
        {
            FirstPick = GetExplicitTarget();
            Targeter.Reset();
            UpdateMouseAxial(out _, out bool valid);
            if (valid)
                Targeter.UpdateTarget(MouseAxial);
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

        // Combines the stashed first pick with the just-confirmed second pick into the
        // final 2-hex target list for a DualDirect spell.
        public List<Hex> GetDualExplicitTarget()
        {
            if (FirstPick == null)
                return null;
            List<Hex> combined = new(FirstPick);
            combined.AddRange(GetExplicitTarget());
            return combined;
        }
    }
}
