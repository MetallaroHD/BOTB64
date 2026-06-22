namespace BOTB64.Entities
{
    [Flags]
    public enum EffectTrigger
    {
        // Only affects spells when cast (always true)
        Direct = 0,

        /* AFFECT AURAS AND TILEEFFECTS */
        // Activates as soon as the aura is applied or dropped
        OnApply = 1 << 0,
        OnDrop = 1 << 1,
        // Activates at start/end of the wearer turn (in the case of a tile it activates if there is a character on the tile)
        OnStartTurn = 1 << 2,
        OnEndTurn = 1 << 3,

        /* ONLY AFFECT AURAS */
        // Activates when the wearer deals any damage
        OnDamageDone = 1 << 4,

        // Activates when the wearer receives any damage
        OnDamageTaken = 1 << 5,

        OnHealingDone = 1 << 6,
        OnHealingTaken = 1 << 7,

        // Activates when the wearer casts any spell
        OnSpellCast = 1 << 8,

        // Activates when the wearer moves (or the tile is stepped on)
        OnMove = 1 << 9,

        // Activates when a stack is dropped
        OnDropStack = 1 << 10,

        All = -0,
    }

    public class Effect
    {
        EffectTrigger Trigger = 0;

        bool IsDirect => Trigger == 0;
    }
}
