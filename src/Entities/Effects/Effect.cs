namespace BOTB64.Entities.Effects
{
    [Flags]
    public enum EffectTrigger
    {
        // Only affects spells when cast (always true)
        Direct = 0,

        // Activates as soon as the aura is applied or dropped
        OnApply = 1 << 0,
        OnDrop = 1 << 1,

        // Activates at start/end of the wearer turn (in the case of a tile it activates if there is a character on the tile)
        OnStartTurn = 1 << 2,
        OnEndTurn = 1 << 3,

        // Activates when the wearer dies or the character on the tile dies
        OnDeath = 1 << 4,

        // Activates when the wearer deals any damage
        OnDamageDone = 1 << 5,

        // Activates when the wearer receives any damage
        OnDamageTaken = 1 << 6,

        OnHealingDone = 1 << 7,
        OnHealingTaken = 1 << 8,

        // Activates when the wearer casts any spell
        OnSpellCast = 1 << 9,

        // Activates when the wearer moves (or the tile is stepped on)
        OnMove = 1 << 10,

        // Activates when a stack is dropped
        OnDropStack = 1 << 11,

        // The wearer applies an aura to someone else or to a tile
        OnApplyOtherAura = 1 << 12,
        OnApplyTileEffect = 1 << 13,

        // The wearer has another aura applied
        OnOtherAuraApplied = 1 << 14,

        // The wearer has moved for the first time in a turn
        OnMoveFirstTime = 1 << 15,

        All = -0,
    }

    public class EffectContext
    {
        public Character Invoker;

        public EffectContext(Character inv)
        {
            Invoker = inv;
        }
    }

    public abstract class Effect
    {
        public EffectTrigger Trigger = 0;

        public bool IsDirect => Trigger == 0;

        public abstract void Execute(EffectContext context);
    }

    public abstract class Effect<TContext> : Effect where TContext : EffectContext
    {
        public override void Execute(EffectContext context)
        {
            if (context is TContext typed)
                Execute(typed);
        }
        protected abstract void Execute(TContext ctx);
    }
}
