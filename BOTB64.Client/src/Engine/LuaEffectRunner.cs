using BOTB64.Engine.Net;
using BOTB64.Entities;
using BOTB64.Runtime;
using MoonSharp.Interpreter;

namespace BOTB64.Engine
{
    public struct TemporaryLuaData
    {
        public int LastDamageDone;
        public bool LastCrit;
    }

    public class LuaEffectRunner
    {
        public readonly Script Lua = new Script();

        public TemporaryLuaData Temp;

        public LuaEffectRunner(Game game) 
        {
            // ACTIONS
            Lua.Globals["Damage"] = (Action<int, int, bool>)((targetID, amount, crit) => { Temp.LastDamageDone = amount; Temp.LastCrit = crit; game.RecordAndApply(new DamageEvent { TargetID = targetID, Amount = amount, Crit = crit }); });
            Lua.Globals["Die"] = (Action<int>)(charId => { game.RecordAndApply(new DeathEvent { CharacterID = charId }); var character = game.FindCharacter(charId); if (character != null) AuraTriggerManager.Execute(new EffectContext(character), EffectTrigger.OnDeath, AuraType.Character | AuraType.Tile); });

            //RNG
            Lua.Globals["Roll"] = (Func<float, bool>)(chance => game.Random() < chance);

            // GETTERS
            Lua.Globals["GetHP"] = (Func<int, int>)(charId => game.FindCharacter(charId)?.CurrentHP ?? 0);
            Lua.Globals["GetAttackPower"] = (Func<int, float>)(charId => game.FindCharacter(charId)?.AttackPower ?? 0);
            Lua.Globals["GetSpellPower"] = (Func<int, float>)(charId => game.FindCharacter(charId)?.SpellPower ?? 0);
            Lua.Globals["GetAutoAttackAP"] = (Func<int, float>)(charId => game.FindCharacter(charId)?.AutoAttackAP ?? 0);
            Lua.Globals["GetAutoAttackSP"] = (Func<int, float>)(charId => game.FindCharacter(charId)?.AutoAttackSP ?? 0);
            Lua.Globals["GetAutoAttackDef"] = (Func<int, float>)(charId => game.FindCharacter(charId)?.AutoAttackDef ?? 0);
            Lua.Globals["GetDefense"] = (Func<int, float>)(charId => game.FindCharacter(charId)?.Defense ?? 0);
            Lua.Globals["GetAutoAttackMDef"] = (Func<int, float>)(charId => game.FindCharacter(charId)?.AutoAttackMDef ?? 0);
            Lua.Globals["GetCritChance"] = (Func<int, float>)(charId => game.FindCharacter(charId)?.Crit ?? 0);
            Lua.Globals["IsAlive"] = (Func<int, bool>)(charId => game.FindCharacter(charId)?.Alive ?? false);
        }

        public void Run(Effect effect, EffectContext context)
        {
            Lua.Globals["Invoker"] = context.Invoker.GameID;
            if (context is SpellCastContext sc)
            {
                Lua.Globals["Caster"] = sc.Caster.GameID;
                Lua.Globals["Targets"] = sc.ExplicitTarget;
            }
            if (context is DirectDamageContext dc)
            {
                Lua.Globals["Attacker"] = dc.DamageDoer.GameID;
                Lua.Globals["Target"] = dc.DamageTaker.GameID;
            }
            Lua.DoString(effect.Script);
        }
    }
}
