using BOTB64.Engine.Net;
using BOTB64.Entities;
using BOTB64.Runtime;
using BOTB64.Shared.Files;
using MoonSharp.Interpreter;

namespace BOTB64.Engine
{
    public class LuaEffectRunner
    {
        public Dictionary<string, string> ScriptCache;

        public readonly Script Lua = new Script();
        private EffectContext CurrentContext;
        private Effect CurrentEffect;

        public LuaEffectRunner(Game game) 
        {
            // ACTIONS
            Lua.Globals["Damage"] = (Action<int, int>)((targetID, amount) => { EffectProcessor.Damage(game, CurrentContext, CurrentEffect, targetID, amount); });
            Lua.Globals["DamageAt"] = (Action<int, int, int>)((q, r, amount) => { Character? tg = game.FindCharacter(q, r); if (tg == null) return; EffectProcessor.Damage(game, CurrentContext, CurrentEffect, tg, amount); });
            Lua.Globals["Die"] = (Action<int>)(charId => { EffectProcessor.Die(game, charId); });

            // OTHER
            Lua.Globals["Roll"] = (Func<float, bool>)(chance => EffectProcessor.Roll(game, chance));
            Lua.Globals["Log"] = (Action<string>)(text => Logger.Log(text));

            // GETTERS
            Lua.Globals["GetHP"] = (Func<int, int>)(charId => game.FindCharacter(charId)?.CurrentHP ?? 0);
            Lua.Globals["GetAttackPower"] = (Func<int, float>)(charId => game.FindCharacter(charId)?.AttackPower ?? 0);
            Lua.Globals["GetSpellPower"] = (Func<int, float>)(charId => game.FindCharacter(charId)?.SpellPower ?? 0);
            Lua.Globals["GetAutoAttackAP"] = (Func<int, float>)(charId => game.FindCharacter(charId)?.AutoAttackAP ?? 0);
            Lua.Globals["GetAutoAttackSP"] = (Func<int, float>)(charId => game.FindCharacter(charId)?.AutoAttackSP ?? 0);
            Lua.Globals["GetDefense"] = (Func<int, float>)(charId => game.FindCharacter(charId)?.Defense ?? 0);
            Lua.Globals["GetCritChance"] = (Func<int, float>)(charId => game.FindCharacter(charId)?.Crit ?? 0);
            Lua.Globals["IsAlive"] = (Func<int, bool>)(charId => game.FindCharacter(charId)?.Alive ?? false);
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
                if (ScriptCache.TryGetValue(effect.Script, out string? code))
                {
                    if (code != null)
                        Lua.DoString(code);
                    else
                        throw new Exception("Invalid Script!");
                }
                else
                {
                    DataFile scriptFile = new DataFile(CommonURIs.ScriptDir + effect.Script + CommonURIs.ScriptExt);
                    string cd = scriptFile.ReadAll();
                    ScriptCache.Add(effect.Script, cd);
                    Lua.DoString(cd);
                }
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
    }
}
