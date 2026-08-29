using BOTB64.Graphics.Animations;
using RL = Raylib_cs;
using BOTB64.Runtime;

namespace BOTB64.Entities
{
    [Flags]
    public enum DispelType
    {
        None = 0,
        Magic = 1 << 0,
        Poison = 1 << 1,
        Bleed = 1 << 2,
        Disease = 1 << 3,
        Hex = 1 << 4
    }

    [Flags]
    public enum AuraSpecialEffect
    {
        None = 0,
        // Prevents movement
        Root = 1 << 0,
        // Prevents all actions
        Stun = 2 << 0,
        // Prevents autoattacking
        Disarm = 3 << 0,
        // Prevents spellcasting
        Silence = 4 << 0,
        // Tags a debuff as a movement-speed slow (not enforced anywhere on its own -
        // just a marker so cleanse effects like Pilaf's Unstoppable Force can find and
        // remove it). Given its own clean bit (1 << 3) so it doesn't collide with the
        // Root/Stun/Disarm/Silence values above, which overlap each other by construction.
        Slow = 1 << 3,
        // Makes ForceMove a no-op against the wearer (Gravitus's Supermassive).
        KnockbackImmune = 1 << 4,
    }

    public class Aura : ExecutableBase, IReadable
    {
        public AuraVfxAnimation Animation { get; set; }
        public RL.Texture2D Icon;

        // --- Base data (does not change during game) --- //
        public int ID = 0;
        public string Name = "";
        public int Duration = 0;
        public int MaxStacks = 0;
        public DispelType Dispel = DispelType.None;
        public AuraSpecialEffect SpecialEffect = AuraSpecialEffect.None;

        public string Tooltip = "";

        // --- Volatile data --- //
        public Character? Owner; //character that applied the spell
        public Character? Wearer; //character that has the spell applied

        public int Remaining = 0;
        public int CurrentStacks = 0;

        public List<Parameter> Parameters = new();

        public Aura Instance()
        {
            Aura ret = new Aura();
            ret.Animation = Animation;
            ret.Icon = Icon;
            ret.ID = ID;
            ret.Name = Name;
            ret.Duration = Duration;
            ret.MaxStacks = MaxStacks;
            ret.Dispel = Dispel;
            ret.SpecialEffect = SpecialEffect;
            ret.Tooltip = Tooltip;
            ret.Parameters = Parameters;    
            ret.Effects = Effects;
            return ret;
        }
    }
}
