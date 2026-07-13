using BOTB64.Engine;
using BOTB64.Engine.Net;
using BOTB64.Entities;
using BOTB64.Runtime;

namespace BOTB64.Entities.Context
{
    public enum DamageSourceType
    {
        Unknown = 0,
        AutoAttack = 1,
        Spell = 2,
        Aura = 3,
    }

    public enum DamageType
    {
        Physical = 0,
        Fire = 1,
        Lightning = 2,
        Frost = 3,
        Nature = 4,
        Light = 5,
        Shadow = 6,
    }

    public class DirectDamageContext : EffectContext
    {
        public int DamageDone;
        public bool Crit;
        public Character DamageDoer;
        public Character DamageTaker;
        public DamageType DamageType = DamageType.Physical;
        public DamageSourceType SourceType = DamageSourceType.Unknown;

        public DirectDamageContext(Character invoker, Character dmger, Character dmged) : base(invoker)
        {
            DamageDoer = dmger;
            DamageTaker = dmged;
        }
    }
}