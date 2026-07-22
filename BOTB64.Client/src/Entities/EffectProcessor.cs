using BOTB64.Engine;
using BOTB64.Engine.Net;
using BOTB64.Runtime;
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

        public static bool Damage(Game game, EffectContext ctx, Effect eff, int targetID, int baseDamage)
        {
            var target = game.FindCharacter(targetID);
            return Damage(game, ctx, eff, target, baseDamage);
        }

        public static bool Damage(Game game, EffectContext ctx, Effect eff, Character target, int baseDamage)
        {
            if (target == null)
                return false;
            var dmgCtx = new DamageContext(ctx.Invoker, ctx.Invoker, target);
            dmgCtx.DamageDone = baseDamage;
            dmgCtx.DamageType = eff.Type;
            dmgCtx.SourceType = eff.Source;
            AuraTriggerManager.Execute(dmgCtx, EffectTrigger.OnPreDamageDealt, AuraType.Character | AuraType.Tile);
            dmgCtx.DamageDone = CalcDamage(dmgCtx.DamageDoer, dmgCtx.DamageTaker, dmgCtx.DamageDone, eff.Scaling);
            dmgCtx.Crit = Roll(game, dmgCtx.DamageDoer.Crit.GetF());
            if (dmgCtx.Crit)
                dmgCtx.DamageDone = (int)(1.5 * dmgCtx.DamageDone);
            game.RecordAndApply(new DamageEvent { TargetID = dmgCtx.DamageTaker.GameID, Amount = dmgCtx.DamageDone, Crit = dmgCtx.Crit });
            game.RecordAndApply(new HealEvent { TargetID = dmgCtx.DamageDoer.GameID, Amount = CalcLifeSteal(dmgCtx.DamageDoer, dmgCtx.DamageDone, eff.Scaling) });
            if (dmgCtx.DamageDone > 0)
            {
                AuraTriggerManager.Execute(ctx, EffectTrigger.OnDamageDone, AuraType.Character | AuraType.Tile);
                if (dmgCtx.DamageTaker.Alive)
                    AuraTriggerManager.Execute(ctx, EffectTrigger.OnDamageTaken, AuraType.Character | AuraType.Tile);
            }
            if (dmgCtx.DamageTaker.CurrentHP <= 0)
                Die(game, dmgCtx.DamageTaker.GameID);
            return true;
        }

        public static bool ApplyAura(Game game, int ownerID, int targetID, int auraID, int stacksToAdd)
        {
            Character? owner = game.FindCharacter(ownerID);
            Character? target = game.FindCharacter(targetID);
            if (owner == null || target == null)
                return false;

            Aura? template = AuraTriggerManager.GetAura(auraID);
            if (template == null)
                return false;

            var existing = target.CurrentAuras.FirstOrDefault(a => a.ID == auraID);
            int currentStacks = existing?.CurrentStacks ?? 0;
            int finalStacks = Math.Min(currentStacks + stacksToAdd, template.MaxStacks);

            var auraCtx = new ApplyAuraContext(owner, owner, target, template);
            game.RecordAndApply(new ApplyAuraEvent { OwnerID = ownerID, TargetID = targetID, AuraID = auraID, FinalStacks = finalStacks });
            AuraTriggerManager.Execute(auraCtx, EffectTrigger.OnApply, AuraType.Character);
            AuraTriggerManager.Execute(auraCtx, EffectTrigger.OnOtherAuraApplied, AuraType.Character);
            return true;
        }

        // EffectProcessor.cs — new method, alongside ApplyAura
        public static bool ApplyTileEffect(Game game, int ownerID, Hex position, int tileEffectID, int duration)
        {
            Character? owner = game.FindCharacter(ownerID);
            var tile = game.GetBoard().GetTile(position);
            if (owner == null || tile == null)
                return false;
            TileEffect? template = AuraTriggerManager.GetTileEffect(tileEffectID); // mirrors GetAura
            if (template == null)
                return false;
            game.RecordAndApply(new ApplyTileEffectEvent { OwnerID = ownerID, Position = position, TileEffectID = tileEffectID, Duration = duration });
            var tileCtx = new EffectContext(owner);
            AuraTriggerManager.Execute(tileCtx, EffectTrigger.OnApply, AuraType.Tile);
            AuraTriggerManager.Execute(tileCtx, EffectTrigger.OnApplyTileEffect, AuraType.Character);
            return true;
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
                    tot -= (1 - atker.ArmorPen.GetF()) * target.Defense.GetI();
                    break;
                case EffectDamageScaling.SpellDamage:
                    tot -= (1 - atker.SpellPen.GetF()) * target.MagicDefense.GetI();
                    break;
                case EffectDamageScaling.Hybrid:
                    tot -= (1 - atker.ArmorPen.GetF()) * target.Defense.GetI() + (1 - atker.SpellPen.GetF()) * target.MagicDefense.GetI();
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
                    ls = atker.LifeSteal.GetF() * bd;
                    break;
                case EffectDamageScaling.SpellDamage:
                    ls = atker.SpellVamp.GetF() * bd;
                    break;
                case EffectDamageScaling.Hybrid:
                    ls = atker.LifeSteal.GetF() * bd + atker.SpellVamp.GetF() * bd;
                    break;
            }
            return (int)Math.Max(0, ls);
        }
    }
}
