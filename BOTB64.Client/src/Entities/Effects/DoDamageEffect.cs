using BOTB64.Engine;
using BOTB64.Engine.Net;

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
        public bool Crit;
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
        protected override void Execute(Game game, DamageContext ctx)
        {
            if (ctx.SourceType == DamageSourceType.AutoAttack)
            {
                int baseDamage = (int)(ctx.DamageDoer.AttackPower * ctx.DamageDoer.AutoAttackAP +
                    ctx.DamageDoer.SpellPower * ctx.DamageDoer.AutoAttackSP -
                    ctx.DamageDoer.AutoAttackDef * ctx.DamageTaker.Defense -
                    ctx.DamageDoer.AutoAttackMDef * ctx.DamageTaker.AutoAttackMDef);

                bool crit = Random.Shared.NextDouble() < ctx.DamageDoer.Crit;

                ctx.Crit = crit;
                ctx.DamageDone = crit ? (int)(baseDamage * 1.5) : baseDamage;

                game.RecordAndApply(new DamageEvent
                {
                    TargetID = ctx.DamageTaker.GameID,
                    Amount = ctx.DamageDone,
                    Crit = crit,
                });

                if (ctx.DamageTaker.CurrentHP <= 0)
                {
                    game.RecordAndApply(new DeathEvent { CharacterID = ctx.DamageTaker.GameID });
                    AuraTriggerManager.Execute(new EffectContext(ctx.DamageTaker), EffectTrigger.OnDeath, AuraType.Character | AuraType.Tile);
                }
            }
        }
    }
}