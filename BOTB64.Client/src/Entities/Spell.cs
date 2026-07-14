using BOTB64.Graphics.Animations;
using BOTB64.Runtime;
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
        public int CostHP = 0;
        public int Preparation = 0;

        public string Tooltip = "";

        // --- Volatile data --- //
        public Character? Owner;

        public int CurrentCD = 0;
        public int CurrentCharges = 0;

        // may only be direct effects
        public List<Parameter> Parameters = new();
    }
}
