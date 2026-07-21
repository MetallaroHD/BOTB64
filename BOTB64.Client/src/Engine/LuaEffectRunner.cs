using BOTB64.Entities;
using BOTB64.Runtime;
using BOTB64.Shared.Files;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Loaders;

namespace BOTB64.Engine
{
    public class LuaEffectRunner
    {
        public Dictionary<string, string> ScriptCache = new();

        public readonly Script Lua = new Script();
        private EffectContext CurrentContext;
        private Effect CurrentEffect;

        public LuaEffectRunner(Game game) 
        {
            // ACTIONS
            Lua.Globals["Damage"] = (Action<int, int>)((targetID, amount) => { EffectProcessor.Damage(game, CurrentContext, CurrentEffect, targetID, amount); });
            Lua.Globals["DamageAt"] = (Action<int, int, int>)((q, r, amount) => { Character? tg = game.FindCharacter(q, r); if (tg == null) return; EffectProcessor.Damage(game, CurrentContext, CurrentEffect, tg, amount); });
            Lua.Globals["Die"] = (Action<int>)(charId => { EffectProcessor.Die(game, charId); });
            Lua.Globals["ApplyAura"] = (Action<int, int, int, int>)((ownerID, targetID, auraID, stacks) => { EffectProcessor.ApplyAura(game, ownerID, targetID, auraID, stacks); });

            // OTHER
            Lua.Globals["Roll"] = (Func<float, bool>)(chance => EffectProcessor.Roll(game, chance));
            Lua.Globals["Log"] = (Action<string>)(text => Logger.Log(text));

            // GETTERS
            Lua.Globals["GetCharacterAt"] = (Func<int, int, int>)((q, r) => { var c = game.FindCharacter(q, r); if (c != null) return c.GameID; return -1; });
            Lua.Globals["GetHP"] = (Func<int, int>)(charId => game.FindCharacter(charId)?.CurrentHP ?? 0);
            Lua.Globals["GetAttackPower"] = (Func<int, float>)(charId => game.FindCharacter(charId)?.AttackPower.GetF() ?? 0);
            Lua.Globals["GetSpellPower"] = (Func<int, float>)(charId => game.FindCharacter(charId)?.SpellPower.GetF() ?? 0);
            Lua.Globals["GetAutoAttackAP"] = (Func<int, float>)(charId => game.FindCharacter(charId)?.AutoAttackAP.GetF() ?? 0);
            Lua.Globals["GetAutoAttackSP"] = (Func<int, float>)(charId => game.FindCharacter(charId)?.AutoAttackSP.GetF() ?? 0);
            Lua.Globals["GetDefense"] = (Func<int, float>)(charId => game.FindCharacter(charId)?.Defense.GetF() ?? 0);
            Lua.Globals["GetCritChance"] = (Func<int, float>)(charId => game.FindCharacter(charId)?.Crit.GetF() ?? 0);
            Lua.Globals["IsAlive"] = (Func<int, bool>)(charId => game.FindCharacter(charId)?.Alive ?? false);

            Lua.Options.ScriptLoader = new ArchiveScriptLoader(LoadScript);
        }

        public void Run(Effect effect, EffectContext context)
        {
            CurrentContext = context;
            CurrentEffect = effect;

            Lua.Globals["Invoker"] = context.Invoker.GameID;
            if (context is SpellCastContext sc)
            {
                Lua.Globals["Caster"] = sc.Caster.GameID;
                Lua.Globals["Targets"] = sc.ExplicitTarget;
            }
            try
            {
                Lua.DoString(LoadScript(effect.Script));
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
        }

        public static void RegisterTypes()
        {
            UserData.RegisterType<Hex>();
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
    }
}
