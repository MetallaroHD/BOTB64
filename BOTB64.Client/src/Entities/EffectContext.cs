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

    public class DamageContext : EffectContext
    {
        public int DamageDone;
        public bool Crit;
        public Character DamageDoer;
        public Character DamageTaker;
        public EffectDamageType DamageType = EffectDamageType.Physical;
        public EffectSourceType SourceType = EffectSourceType.Unknown;

        public DamageContext(Character invoker, Character dmger, Character dmged) : base(invoker)
        {
            DamageDoer = dmger;
            DamageTaker = dmged;
        }
    }
}
