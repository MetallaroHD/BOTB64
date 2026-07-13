using BOTB64.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOTB64.Entities
{
    public class EffectContext
    {
        public Character Invoker { get; set; }

        public EffectContext(Character invoker)
        {
            Invoker = invoker;
        }
    }

    public class SpellCastContext : EffectContext
    {
        public Character Caster { get; set; }

        public List<Hex> ExplicitTarget { get; set; }

        public SpellCastContext(Character invoker, List<Hex> expl) : base(invoker)
        {
            Caster = Invoker;
            ExplicitTarget = expl;
        }
    }

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
