namespace BOTB64.Entities.Effects
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

    public class DamageContext : EffectContext
    {
        public int DamageDone;
        public Character DamageDoer;
        public Character DamageTaker;
        public DamageType DamageType = DamageType.Physical;
        public DamageSourceType SourceType = DamageSourceType.Unknown;

        public DamageContext(Character invoker, Character dmger, Character dmged) : base(invoker)
        {
            DamageDoer = dmger;
            DamageTaker = dmged;
        }
    }

    public class DoDamageEffect : Effect<DamageContext>
    {
        protected override void Execute(DamageContext ctx)
        {
            if (ctx.SourceType == DamageSourceType.AutoAttack)
            {
                int baseDamage = (int)(ctx.DamageDoer.AttackPower * ctx.DamageDoer.AutoAttackAP +
                    ctx.DamageDoer.SpellPower * ctx.DamageDoer.AutoAttackSP -
                    ctx.DamageDoer.AutoAttackDef * ctx.DamageTaker.Defense -
                    ctx.DamageDoer.AutoAttackMDef * ctx.DamageTaker.AutoAttackMDef);
                ctx.DamageDone = baseDamage;
                ctx.DamageTaker.TakeDamage(ctx);
            }
        }
    }
}