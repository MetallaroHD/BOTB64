using BOTB64.Engine;
using BOTB64.Engine.Net;
using System;

namespace BOTB64.Entities
{
    /* Contains the internals for all effects in the game */
    public static class EffectProcessor
    {
        public static bool Roll(Game game, float thresh)
        {
            return game.Random() < thresh;
        }

        public static void Damage(Game game, EffectContext ctx, int targetID, int baseDamage, EffectDamageType dType, EffectSourceType sType, EffectDamageScaling scal)
        {
            var target = game.FindCharacter(targetID);
            Damage(game, ctx, target, baseDamage, dType, sType, scal);
        }

        public static void Damage(Game game, EffectContext ctx, Character target, int baseDamage, EffectDamageType dType, EffectSourceType sType, EffectDamageScaling scal)
        {
            if (target == null)
                return;
            var dmgCtx = new DamageContext(ctx.Invoker, ctx.Invoker, target);
            dmgCtx.DamageDone = baseDamage;
            dmgCtx.DamageType = dType;
            dmgCtx.SourceType = sType;
            AuraTriggerManager.Execute(dmgCtx, EffectTrigger.OnPreDamageDealt, AuraType.Character | AuraType.Tile);
            dmgCtx.DamageDone = CalcDamage(dmgCtx.DamageDoer, dmgCtx.DamageTaker, dmgCtx.DamageDone, scal);
            dmgCtx.Crit = Roll(game, dmgCtx.DamageDoer.Crit);
            if (dmgCtx.Crit)
                dmgCtx.DamageDone = (int)(1.5 * dmgCtx.DamageDone);
            game.RecordAndApply(new DamageEvent { TargetID = dmgCtx.DamageTaker.GameID, Amount = dmgCtx.DamageDone, Crit = dmgCtx.Crit });
            game.RecordAndApply(new HealEvent { TargetID = dmgCtx.DamageDoer.GameID, Amount = CalcLifeSteal(dmgCtx.DamageDoer, dmgCtx.DamageDone, scal) });
            if (dmgCtx.DamageDone > 0)
            {
                AuraTriggerManager.Execute(ctx, EffectTrigger.OnDamageDone, AuraType.Character | AuraType.Tile);
                if (dmgCtx.DamageTaker.Alive)
                    AuraTriggerManager.Execute(ctx, EffectTrigger.OnDamageTaken, AuraType.Character | AuraType.Tile);
            }
            if (dmgCtx.DamageTaker.CurrentHP <= 0)
                Die(game, dmgCtx.DamageTaker.GameID);
        }

        public static void Die(Game game, int charID)
        {
            game.RecordAndApply(new DeathEvent { CharacterID = charID }); 
            var character = game.FindCharacter(charID); 
            if (character != null) 
                AuraTriggerManager.Execute(new EffectContext(character), EffectTrigger.OnDeath, AuraType.Character | AuraType.Tile);
        }

        private static int CalcDamage(Character atker, Character target, int bd, EffectDamageScaling scal)
        {
            float tot = bd;
            switch (scal)
            {
                case EffectDamageScaling.None:
                    break;
                case EffectDamageScaling.AttackDamage:
                    tot -= (1 - atker.ArmorPen) * target.Defense;
                    break;
                case EffectDamageScaling.SpellDamage:
                    tot -= (1 - atker.SpellPen) * target.MagicDefense;
                    break;
                case EffectDamageScaling.Hybrid:
                    tot -= (1 - atker.ArmorPen) * target.Defense + (1 - atker.SpellPen) * target.MagicDefense;
                    break;
            }
            return (int)Math.Max(1, tot);
        }
        private static int CalcLifeSteal(Character atker, int bd, EffectDamageScaling scal)
        {
            float ls = 0;
            switch (scal)
            {
                case EffectDamageScaling.None:
                    break;
                case EffectDamageScaling.AttackDamage:
                    ls = atker.LifeSteal * bd;
                    break;
                case EffectDamageScaling.SpellDamage:
                    ls = atker.SpellVamp * bd;
                    break;
                case EffectDamageScaling.Hybrid:
                    ls = atker.LifeSteal * bd + atker.SpellVamp * bd;
                    break;
            }
            return (int)Math.Max(0, ls);
        }
    }
}
