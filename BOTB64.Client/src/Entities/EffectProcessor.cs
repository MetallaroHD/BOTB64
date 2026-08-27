using BOTB64.Engine;
using BOTB64.Engine.Net;
using BOTB64.Runtime;

namespace BOTB64.Entities
{
    /* Contains the internals for all effects in the game */
    public static class EffectProcessor
    {
        public static float Random(Game game, float min, float max)
        {
            double rng = game.Random();
            return (float)rng * max + (1 - (float)rng) * min;
        }

        public static bool Roll(Game game, float thresh)
        {
            return game.Random() < thresh;
        }

        public static bool CheckLOS(Game game, int from, int to)
        {
            Character? fromChar = game.FindCharacter(from);
            Character? toChar = game.FindCharacter(to);

            if (fromChar == null || toChar == null)
                return false;

            var beam = HexAlgo.Beam(fromChar.Position, toChar.Position);
            foreach (Hex h in beam)
            {
                Tile? t = game.GetBoard().GetTile(h);
                if (t == null)
                    return false;
                if (!t.AllowsLos())
                    return false;
            }

            return true;
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

        public static bool ForceMove(Game game, int charID, Hex destination)
        {
            Character? character = game.FindCharacter(charID);
            if (character == null)
                return false;

            Tile? tile = game.GetBoard().GetTile(destination);
            if (tile == null || !tile.IsPassable())
                return false;

            game.RecordAndApply(new ForcedMoveEvent { CharacterID = charID, Step = destination });

            var ctx = new EffectContext(character);
            AuraTriggerManager.Execute(ctx, EffectTrigger.OnMove, AuraType.Character | AuraType.Tile);

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

        public static void SetAuraParam(Game game, int wearerId, int auraId, string key, float value)
        {
            var wearer = game.FindCharacter(wearerId);
            var aura = wearer?.CurrentAuras.FirstOrDefault(a => a.ID == auraId);
            if (aura == null) return;
            game.RecordAndApply(new AuraParamSetEvent { CharacterID = wearerId, AuraID = auraId, Key = key, Value = value });
        }

        public static float GetAuraParam(Game game, int wearerId, int auraId, string key)
        {
            var wearer = game.FindCharacter(wearerId);
            var aura = wearer?.CurrentAuras.FirstOrDefault(a => a.ID == auraId);
            var param = aura?.Parameters.FirstOrDefault(p => p.Name == key);
            return param?.GetFloat(0f) ?? 0f; // read-only — no event needed, same tier as GetHP
        }

        public static bool ModifyStat(Game game, int characterId, string statName, float addDelta, float mulDelta)
        {
            var character = game.FindCharacter(characterId);
            if (character == null) return false;

            if (!Enum.TryParse<StatType>(statName, ignoreCase: true, out var stat))
            {
                Logger.Log($"ModifyStat: unknown stat '{statName}'");
                return false;
            }

            game.RecordAndApply(new StatModifiedEvent
            {
                CharacterID = characterId,
                Stat = stat,
                AddDelta = addDelta,
                MulDelta = mulDelta
            });
            return true;
        }

        public static void DropAura(Game game, int charId, int auraId, int stacks)
        {
            Character? character = game.FindCharacter(charId);
            if(character == null) return;

            var existing = character.CurrentAuras.FirstOrDefault(a => a.ID == auraId);

            if(existing == null) 
                return;

            int currentStacks = existing?.CurrentStacks ?? 0;
            int finalStacks = Math.Min(currentStacks - stacks, existing.MaxStacks);

            var auraCtx = new ApplyAuraContext(existing.Wearer, existing.Owner, existing.Wearer, existing);
            game.RecordAndApply(new ApplyAuraEvent { OwnerID = existing.Owner.GameID, TargetID = existing.Wearer.GameID, AuraID = existing.ID, FinalStacks = finalStacks });
            AuraTriggerManager.Execute(auraCtx, EffectTrigger.OnDropStack, AuraType.Character);
            if(finalStacks <= 0)
                AuraTriggerManager.Execute(auraCtx, EffectTrigger.OnDrop, AuraType.Character);
        }

        public static void SpendAction(Game game, int characterID, bool fast)
        {
            game.RecordAndApply(new ActionSpentEvent { CharacterID = characterID, FastAction = fast });
        }

        public static bool IsEnemy(Game game, int charID1, int charID2)
        {
            Character? a = game.FindCharacter(charID1);
            Character? b = game.FindCharacter(charID2);
            if (a == null || b == null)
                return false;
            return a.Faction != b.Faction;
        }

        public static int HexDistance(int q1, int r1, int q2, int r2)
        {
            return HexAlgo.HexDistance(new Hex(q1, r1), new Hex(q2, r2));
        }

        public static List<Hex> GetHexesInRadius(int q, int r, int radius)
        {
            var center = new Hex(q, r);
            var result = new List<Hex>();
            for (int dq = -radius; dq <= radius; dq++)
                for (int dr = -radius; dr <= radius; dr++)
                {
                    var h = new Hex(q + dq, r + dr);
                    if (HexAlgo.HexDistance(center, h) <= radius)
                        result.Add(h);
                }
            return result;
        }

        public static List<Hex> GetLine(int fromQ, int fromR, int toQ, int toR)
        {
            return HexAlgo.Beam(new Hex(fromQ, fromR), new Hex(toQ, toR));
        }

        public static bool TileBlocksLos(Game game, int q, int r)
        {
            Tile? tile = game.GetBoard().GetTile(new Hex(q, r));
            return tile == null || !tile.AllowsLos();
        }

        public static bool MoveTileEffect(Game game, int ownerID, int fromQ, int fromR, int toQ, int toR, int tileEffectID, int duration)
        {
            var fromHex = new Hex(fromQ, fromR);
            var fromTile = game.GetBoard().GetTile(fromHex);
            if (fromTile != null && fromTile.Effects.Any(e => e.ID == tileEffectID))
                game.RecordAndApply(new TileEffectExpiredEvent { Position = fromHex, TileEffectID = tileEffectID });

            return ApplyTileEffect(game, ownerID, new Hex(toQ, toR), tileEffectID, duration);
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
