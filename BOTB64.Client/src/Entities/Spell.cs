using BOTB64.Engine;
using BOTB64.Graphics.Animations;
using BOTB64.Runtime;
using System.Text;
using RL = Raylib_cs;

namespace BOTB64.Entities
{
    public class Spell : ExecutableBase, IReadable
    {
        public RL.Texture2D Icon;
        public SpellVfxAnimation? Animation;

        // --- Base data (does not change during game) --- //
        public int ID = 0;
        public string Name = "";
        public int Range = 0;
        public int Cooldown = 0;
        public int Charges = 0;
        public int Cost = 0;
        public float CostPct = 0;
        public int Preparation = 0;
        public TargetingType ExplicitTarget = TargetingType.Direct;
        // 0 = normal action, 1+ = cast, -1 = fast, -2 = free
        public int CastTime = 0;

        public string Tooltip = "";

        public bool IsPassive => Effects.Count < 1;

        // --- Volatile data --- //
        public Character? Owner;

        public int CurrentCD = 0;
        public int CurrentCharges = 0;

        public List<Parameter> Parameters = new();
        public LuaResult Cast(Game game, SpellCastContext ctx)
        {
            LuaResult ret = new LuaResult { Success = false, ErrorMessage = "Spell is passive!" };
            if (IsPassive)
                return ret;

            var runner = game.GetLuaRunner();
            ret = runner.Run(Effects[0], ctx);

            for (int i = 1; i <Effects.Count; i++)
                runner.Run(Effects[i], ctx);

            return ret;
        }
    }
}
