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

            // OTHER
            Lua.Globals["Random"] = (Func<float, float, float>)((min, max) => EffectProcessor.Random(game, min, max));
            Lua.Globals["Roll"] = (Func<float, bool>)(chance => EffectProcessor.Roll(game, chance));
            Lua.Globals["Log"] = (Action<string>)(text => Logger.Log(text));

            // GETTERS
            Lua.Globals["IsDirect"] = (Func<bool>)(() => CurrentEffect.IsDirect);
            Lua.Globals["HasTrigger"] = (Func<EffectTrigger, bool>)(t => { return CurrentEffect.Trigger.HasFlag(t); });
            Lua.Globals["GetCharacterAt"] = (Func<int, int, int>)((q, r) => { var c = game.FindCharacter(q, r); if (c != null) return c.GameID; return -1; });
            Lua.Globals["GetHP"] = (Func<int, int>)(charId => game.FindCharacter(charId)?.CurrentHP ?? 0);
            Lua.Globals["GetAttackPower"] = (Func<int, float>)(charId => game.FindCharacter(charId)?.AttackPower.GetF() ?? 0);
            Lua.Globals["GetSpellPower"] = (Func<int, float>)(charId => game.FindCharacter(charId)?.SpellPower.GetF() ?? 0);
            Lua.Globals["GetAutoAttackAP"] = (Func<int, float>)(charId => game.FindCharacter(charId)?.AutoAttackAP.GetF() ?? 0);
            Lua.Globals["GetAutoAttackSP"] = (Func<int, float>)(charId => game.FindCharacter(charId)?.AutoAttackSP.GetF() ?? 0);
            Lua.Globals["GetDefense"] = (Func<int, float>)(charId => game.FindCharacter(charId)?.Defense.GetF() ?? 0);
            Lua.Globals["GetCritChance"] = (Func<int, float>)(charId => game.FindCharacter(charId)?.Crit.GetF() ?? 0);
            Lua.Globals["GetPosition"] = (Func<int, Hex>)(charId => game.FindCharacter(charId)?.Position ?? new Hex(-999, -999));
            Lua.Globals["IsAlive"] = (Func<int, bool>)(charId => game.FindCharacter(charId)?.Alive ?? false);
            Lua.Globals["IsRooted"] = (Func<int, bool>)(charId => game.FindCharacter(charId)?.HasSpecialEffect(AuraSpecialEffect.Root) ?? false);
            Lua.Globals["IsStunned"] = (Func<int, bool>)(charId => game.FindCharacter(charId)?.HasSpecialEffect(AuraSpecialEffect.Stun) ?? false);
            Lua.Globals["IsSilenced"] = (Func<int, bool>)(charId => game.FindCharacter(charId)?.HasSpecialEffect(AuraSpecialEffect.Silence) ?? false);
            Lua.Globals["IsDisarmed"] = (Func<int, bool>)(charId => game.FindCharacter(charId)?.HasSpecialEffect(AuraSpecialEffect.Disarm) ?? false);
            Lua.Globals["HasLineOfSight"] = (Func<int, int, bool>)((fromChar, toChar) => EffectProcessor.CheckLOS(game, fromChar, toChar));

            // TYPES
            Lua.Globals["EffectTrigger"] = UserData.CreateStatic<EffectTrigger>();

            Lua.Options.ScriptLoader = new ArchiveScriptLoader(LoadScript);
        }

        public LuaResult Run(Effect effect, EffectContext context)
        {
            LuaResult ret = new LuaResult { Success = false, ErrorMessage = "Generic script error." };
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
            try
            {
                ret = RunCode(LoadScript(effect.Script));
            }
            catch (Exception e)
            {
                Console.WriteLine("Lua exception: " + e.Message);
            }
            finally 
            {
                CurrentContext = null;
                CurrentEffect = null;
            }

            return ret;
        }

        public static void RegisterTypes()
        {
            UserData.RegisterType<Hex>();
            UserData.RegisterType<EffectTrigger>();
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
