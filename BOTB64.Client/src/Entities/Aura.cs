using BOTB64.Graphics.Animations;
using RL = Raylib_cs;
using BOTB64.Runtime;

namespace BOTB64.Entities
{
    [Flags]
    public enum DispelType
    {
        None = 0,
        Magic = 1 << 0,
        Poison = 1 << 1,
        Bleed = 1 << 2,
        Disease = 1 << 3,
        Hex = 1 << 4
    }

    public class Aura : ExecutableBase, IReadable
    {
        public AuraVfxAnimation Animation { get; set; }
        public RL.Texture2D Icon;

        // --- Base data (does not change during game) --- //
        public int ID = 0;
        public string Name = "";
        public int Duration = 0;
        public int MaxStacks = 0;
        public DispelType Dispel = DispelType.None;

        public string Tooltip = "";

        // --- Volatile data --- //
        public Character? Owner; //character that applied the spell
        public Character? Wearer; //character that has the spell applied

        public int Remaining = 0;
        public int CurrentStacks = 0;

        public List<Parameter> Parameters = new();
    }
}
