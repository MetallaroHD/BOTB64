using BOTB64.Runtime;

namespace BOTB64.Entities
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

        // When the spell or attack crits
        OnCrit = 1 << 16,

        // Just before damage application
        OnPreDamageDealt = 1 << 17,

        // On autoattack done
        OnAutoAttack = 1 << 18,

        All = -0,
    }
    public enum EffectSourceType
    {
        Unknown = 0,
        AutoAttack = 1,
        Spell = 2,
        Aura = 3,
        TileEffect = 4,
    }

    public enum EffectDamageType
    {
        None = 0,
        Physical = 1,
        Fire = 2,
        Lightning = 3,
        Frost = 4,
        Nature = 5,
        Light = 6,
        Shadow = 7,
    }

    public enum EffectDamageScaling
    {
        None = 0,
        AttackDamage = 1,
        SpellDamage = 2,
        Hybrid = 4,
    }

    public class Effect
    {
        // Just the name of the script - no extension
        public string Script = "";
        public EffectTrigger Trigger = 0;
        public EffectSourceType Source = EffectSourceType.Unknown;
        public EffectDamageType Type = EffectDamageType.None;
        public EffectDamageScaling Scaling = EffectDamageScaling.None;

        public bool IsDirect => Trigger == 0;
    }
}
