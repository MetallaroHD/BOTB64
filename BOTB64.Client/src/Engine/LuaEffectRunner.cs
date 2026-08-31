using BOTB64.Entities;
using BOTB64.Runtime;
using BOTB64.Shared.Files;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Loaders;

namespace BOTB64.Engine
{
    public struct LuaResult
    {
        public bool Success;
        public string ErrorMessage;
    }

    public class LuaEffectRunner
    {
        public Dictionary<string, string> ScriptCache = new();

        public readonly Script Lua = new Script();
        private EffectContext CurrentContext;
        private Effect CurrentEffect;

        public LuaEffectRunner(Game game) 
        {
            // ACTIONS
            Lua.Globals["Damage"] = (Func<int, int, bool>)((targetID, amount) => { return EffectProcessor.Damage(game, CurrentContext, CurrentEffect, targetID, amount); });
            Lua.Globals["DamageAt"] = (Func<int, int, int, bool>)((q, r, amount) => { Character? tg = game.FindCharacter(q, r); if (tg == null) return false; return EffectProcessor.Damage(game, CurrentContext, CurrentEffect, tg, amount); });
            Lua.Globals["Die"] = (Action<int>)(charId => { EffectProcessor.Die(game, charId); });
            Lua.Globals["ApplyAura"] = (Func<int, int, int, int, bool>)((ownerID, targetID, auraID, stacks) => { return EffectProcessor.ApplyAura(game, ownerID, targetID, auraID, stacks); });
            Lua.Globals["ApplyTileEffect"] = (Func<int, int, int, int, int, bool>)((ownerID, q, r, tileEffectID, duration) => EffectProcessor.ApplyTileEffect(game, ownerID, new Hex(q, r), tileEffectID, duration));
            Lua.Globals["ModifyStat"] = (Func<int, string, float, float, bool>)((charId, statName, addDelta, mulDelta) => EffectProcessor.ModifyStat(game, charId, statName, addDelta, mulDelta));
            Lua.Globals["SetAuraParam"] = (Action<int, int, string, float>)((wearerId, auraId, key, value) => EffectProcessor.SetAuraParam(game, wearerId, auraId, key, value));
            Lua.Globals["GetAuraParam"] = (Func<int, int, string, float>)((wearerId, auraId, key) => EffectProcessor.GetAuraParam(game, wearerId, auraId, key));
            Lua.Globals["SpendAction"] = (Action<int, bool>)((charId, fast) => EffectProcessor.SpendAction(game, charId, fast));
            Lua.Globals["DropAura"] = (Action<int, int, int>)((charId, auraId, stacks) => EffectProcessor.DropAura(game, charId, auraId, stacks));
            Lua.Globals["ForceMove"] = (Func<int, int, int, bool>)((charId, q, r) => EffectProcessor.ForceMove(game, charId, new Hex(q, r)));
            Lua.Globals["PullToward"] = (Func<int, int, int, int, bool>)((charId, towardQ, towardR, steps) => EffectProcessor.PullToward(game, charId, towardQ, towardR, steps));
            Lua.Globals["MoveTileEffect"] = (Func<int, int, int, int, int, int, int, bool>)((ownerID, fromQ, fromR, toQ, toR, tileEffectID, duration) => EffectProcessor.MoveTileEffect(game, ownerID, fromQ, fromR, toQ, toR, tileEffectID, duration));
            Lua.Globals["DamageAs"] = (Func<int, int, int, bool>)((doerID, targetID, amount) => EffectProcessor.DamageAs(game, CurrentEffect, doerID, targetID, amount));
            Lua.Globals["SetTileEffectParam"] = (Action<int, int, int, string, float>)((q, r, tileEffectId, key, value) => EffectProcessor.SetTileEffectParam(game, q, r, tileEffectId, key, value));
            Lua.Globals["GetTileEffectParam"] = (Func<int, int, int, string, float>)((q, r, tileEffectId, key) => EffectProcessor.GetTileEffectParam(game, q, r, tileEffectId, key));
            Lua.Globals["ModifyResource"] = (Action<int, int>)((charId, delta) => EffectProcessor.ModifyResource(game, charId, delta));
            Lua.Globals["GetResource"] = (Func<int, int>)(charId => game.FindCharacter(charId)?.CurrentResource ?? 0);
            Lua.Globals["Heal"] = (Action<int, int>)((charId, amount) => EffectProcessor.Heal(game, charId, amount));
            // Only meaningful from an OnPreDamageDealt/OnPreDamageTaken script - scales the
            // in-flight damage amount before mitigation/crit are applied.
            Lua.Globals["ScaleDamage"] = (Action<float>)(mult => { if (CurrentContext is DamageContext dc) dc.DamageDone = (int)(dc.DamageDone * mult); });
            Lua.Globals["FindTileEffectPositions"] = (Func<int, List<Hex>>)(tileEffectId => EffectProcessor.FindTileEffectPositions(game, tileEffectId));
            Lua.Globals["RemoveTileEffect"] = (Action<int, int, int>)((q, r, tileEffectId) => EffectProcessor.RemoveTileEffect(game, q, r, tileEffectId));
            Lua.Globals["GetAuraStacks"] = (Func<int, int, int>)((charId, auraId) => EffectProcessor.GetAuraStacks(game, charId, auraId));
            Lua.Globals["HasTileEffect"] = (Func<int, int, int, bool>)((q, r, tileEffectId) => EffectProcessor.HasTileEffect(game, q, r, tileEffectId));
            Lua.Globals["IsWall"] = (Func<int, int, bool>)((q, r) => EffectProcessor.IsWall(game, q, r));
            Lua.Globals["PayHealthCost"] = (Action<int, int>)((charId, amount) => EffectProcessor.PayHealthCost(game, charId, amount));
            Lua.Globals["RemoveAurasWithSpecialEffect"] = (Action<int, AuraSpecialEffect>)((charId, effect) => EffectProcessor.RemoveAurasWithSpecialEffect(game, charId, effect));
            Lua.Globals["PlayVfxInstant"] = (Action<string, int, int>)((vfxId, q, r) => EffectProcessor.PlayVfxInstant(game, vfxId, q, r));
            Lua.Globals["PlayVfxProjectile"] = (Action<string, int, int, int, int>)((vfxId, fromQ, fromR, toQ, toR) => EffectProcessor.PlayVfxProjectile(game, vfxId, fromQ, fromR, toQ, toR));
            Lua.Globals["PlayVfxBeam"] = (Action<string, int, int, int, int>)((vfxId, fromQ, fromR, toQ, toR) => EffectProcessor.PlayVfxBeam(game, vfxId, fromQ, fromR, toQ, toR));

            // OTHER
            Lua.Globals["Random"] = (Func<float, float, float>)((min, max) => EffectProcessor.Random(game, min, max));
            Lua.Globals["Roll"] = (Func<float, bool>)(chance => EffectProcessor.Roll(game, chance));
            Lua.Globals["Log"] = (Action<string>)(text => Logger.Log(text));

            // GETTERS
            Lua.Globals["IsDirect"] = (Func<bool>)(() => CurrentEffect.IsDirect);
            Lua.Globals["HasTrigger"] = (Func<EffectTrigger, bool>)(t => { return CurrentEffect.Trigger.HasFlag(t); });
            Lua.Globals["GetCharacterAt"] = (Func<int, int, int>)((q, r) => { var c = game.FindCharacter(q, r); if (c != null) return c.GameID; return -1; });
            Lua.Globals["GetHP"] = (Func<int, int>)(charId => game.FindCharacter(charId)?.CurrentHP ?? 0);
            Lua.Globals["GetMaxHP"] = (Func<int, int>)(charId => game.FindCharacter(charId)?.MaxHP.GetI() ?? 0);
            Lua.Globals["GetAttackPower"] = (Func<int, float>)(charId => game.FindCharacter(charId)?.AttackPower.GetF() ?? 0);
            Lua.Globals["GetSpellPower"] = (Func<int, float>)(charId => game.FindCharacter(charId)?.SpellPower.GetF() ?? 0);
            Lua.Globals["GetAutoAttackAP"] = (Func<int, float>)(charId => game.FindCharacter(charId)?.AutoAttackAP.GetF() ?? 0);
            Lua.Globals["GetAutoAttackSP"] = (Func<int, float>)(charId => game.FindCharacter(charId)?.AutoAttackSP.GetF() ?? 0);
            Lua.Globals["GetDefense"] = (Func<int, float>)(charId => game.FindCharacter(charId)?.Defense.GetF() ?? 0);
            Lua.Globals["GetMagicDefense"] = (Func<int, float>)(charId => game.FindCharacter(charId)?.MagicDefense.GetF() ?? 0);
            Lua.Globals["GetCritChance"] = (Func<int, float>)(charId => game.FindCharacter(charId)?.Crit.GetF() ?? 0);
            Lua.Globals["GetPosition"] = (Func<int, Hex>)(charId => game.FindCharacter(charId)?.Position ?? new Hex(-999, -999));
            Lua.Globals["IsAlive"] = (Func<int, bool>)(charId => game.FindCharacter(charId)?.Alive ?? false);
            Lua.Globals["IsRooted"] = (Func<int, bool>)(charId => game.FindCharacter(charId)?.HasSpecialEffect(AuraSpecialEffect.Root) ?? false);
            Lua.Globals["IsStunned"] = (Func<int, bool>)(charId => game.FindCharacter(charId)?.HasSpecialEffect(AuraSpecialEffect.Stun) ?? false);
            Lua.Globals["IsSilenced"] = (Func<int, bool>)(charId => game.FindCharacter(charId)?.HasSpecialEffect(AuraSpecialEffect.Silence) ?? false);
            Lua.Globals["IsDisarmed"] = (Func<int, bool>)(charId => game.FindCharacter(charId)?.HasSpecialEffect(AuraSpecialEffect.Disarm) ?? false);
            Lua.Globals["HasLineOfSight"] = (Func<int, int, bool>)((fromChar, toChar) => EffectProcessor.CheckLOS(game, fromChar, toChar));
            Lua.Globals["GetSpeed"] = (Func<int, float>)(charId => game.FindCharacter(charId)?.Speed.GetF() ?? 0);
            Lua.Globals["IsEnemy"] = (Func<int, int, bool>)((charId1, charId2) => EffectProcessor.IsEnemy(game, charId1, charId2));
            Lua.Globals["HexDistance"] = (Func<int, int, int, int, int>)((q1, r1, q2, r2) => EffectProcessor.HexDistance(q1, r1, q2, r2));
            Lua.Globals["GetHexesInRadius"] = (Func<int, int, int, List<Hex>>)((q, r, radius) => EffectProcessor.GetHexesInRadius(q, r, radius));
            Lua.Globals["GetLine"] = (Func<int, int, int, int, List<Hex>>)((fromQ, fromR, toQ, toR) => EffectProcessor.GetLine(fromQ, fromR, toQ, toR));
            Lua.Globals["TileBlocksLos"] = (Func<int, int, bool>)((q, r) => EffectProcessor.TileBlocksLos(game, q, r));
            Lua.Globals["GetAllCharacterIDs"] = (Func<List<int>>)(() => game.GetAllCharacterIDs());

            // TYPES
            Lua.Globals["EffectTrigger"] = UserData.CreateStatic<EffectTrigger>();
            Lua.Globals["EffectDamageType"] = UserData.CreateStatic<EffectDamageType>();
            Lua.Globals["AuraSpecialEffect"] = UserData.CreateStatic<AuraSpecialEffect>();

            Lua.Options.ScriptLoader = new ArchiveScriptLoader(LoadScript);
        }

        public LuaResult Run(Effect effect, EffectContext context)
        {
            LuaResult ret = new LuaResult { Success = false, ErrorMessage = "Generic script error." };

            // Effects can trigger other effects re-entrantly (e.g. ApplyAura firing an OnApply
            // script mid-script), which run on this same Script/globals. Save and restore this
            // frame's state so a nested Run() can't clobber the outer call's in-flight globals.
            var prevContext = CurrentContext;
            var prevEffect = CurrentEffect;
            var prevSuccess = Lua.Globals["Success"];
            var prevFail = Lua.Globals["Fail"];
            var prevInvoker = Lua.Globals["Invoker"];
            var prevCaster = Lua.Globals["Caster"];
            var prevTargets = Lua.Globals["Targets"];
            var prevPosition = Lua.Globals["Position"];
            var prevAttacker = Lua.Globals["Attacker"];
            var prevDamageTarget = Lua.Globals["DamageTarget"];
            var prevDamageAmount = Lua.Globals["DamageAmount"];
            var prevIsCrit = Lua.Globals["IsCrit"];
            var prevDamageType = Lua.Globals["DamageType"];

            CurrentContext = context;
            CurrentEffect = effect;

            Lua.Globals["Invoker"] = context.Invoker.GameID;
            if (context is SpellCastContext sc)
            {
                Lua.Globals["Caster"] = sc.Caster.GameID;
                Lua.Globals["Targets"] = sc.ExplicitTarget;
            }
            else if (context is TileEffectContext tc)
            {
                Lua.Globals["Position"] = tc.Position;
            }
            if (context is DamageContext dc)
            {
                Lua.Globals["Attacker"] = dc.DamageDoer.GameID;
                Lua.Globals["DamageTarget"] = dc.DamageTaker.GameID;
                Lua.Globals["DamageAmount"] = dc.DamageDone;
                Lua.Globals["IsCrit"] = dc.Crit;
                Lua.Globals["DamageType"] = dc.DamageType;
            }
            try
            {
                ret = RunCode(LoadScript(effect.Script));
            }
            catch (Exception e)
            {
                ret.Success = false;
                ret.ErrorMessage = e.Message;
                Console.WriteLine("Lua exception: " + e.Message);
            }
            finally
            {
                CurrentContext = prevContext;
                CurrentEffect = prevEffect;
                Lua.Globals["Success"] = prevSuccess;
                Lua.Globals["Fail"] = prevFail;
                Lua.Globals["Invoker"] = prevInvoker;
                Lua.Globals["Caster"] = prevCaster;
                Lua.Globals["Targets"] = prevTargets;
                Lua.Globals["Position"] = prevPosition;
                Lua.Globals["Attacker"] = prevAttacker;
                Lua.Globals["DamageTarget"] = prevDamageTarget;
                Lua.Globals["DamageAmount"] = prevDamageAmount;
                Lua.Globals["IsCrit"] = prevIsCrit;
                Lua.Globals["DamageType"] = prevDamageType;
            }

            return ret;
        }

        public static void RegisterTypes()
        {
            UserData.RegisterType<Hex>();
            UserData.RegisterType<EffectTrigger>();
            UserData.RegisterType<EffectDamageType>();
            UserData.RegisterType<AuraSpecialEffect>();
        }

        public void End()
        {
            ScriptCache.Clear();
        }

        private string LoadScript(string module)
        {
            module = Path.GetFileNameWithoutExtension(module);

            if (ScriptCache.TryGetValue(module, out var code))
                return code;

            DataFile scriptFile = new DataFile(CommonURIs.ScriptDir + module + CommonURIs.ScriptExt);

            code = scriptFile.ReadAll();

            if (string.IsNullOrWhiteSpace(code))
                throw new FileNotFoundException($"Lua module '{module}' not found.");

            ScriptCache[module] = code;

            return code;
        }

        private LuaResult RunCode(string code)
        {
            LuaResult ret = new LuaResult { Success = false, ErrorMessage = "Script is empty!" };

            if (code == "")
                return ret;

            Lua.Globals["Success"] = (Action)(() => ret.Success = true);
            Lua.Globals["Fail"] = (Action<string>)(errorMessage => { ret.Success = false; ret.ErrorMessage = errorMessage; });

            Lua.DoString(code);
            return ret;
        }
    }
}
