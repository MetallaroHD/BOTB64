using BOTB64.Entities.Effects;
using System.Reflection.Metadata;

namespace BOTB64.Entities
{
    [Flags]
    public enum DispelType
    {
        None = 0,
        Magic = 1 << 0,
        Poison = 1 << 1,
        Bleed = 1 << 2,
        Root = 1 << 3,
        Slow = 1 << 4
    }

    public class Aura : ExecutableBase, IReadable
    {
        // add visuals later 
        public Character? Owner;
        public Character? Wearer;

        public int ID = 0;
        public string Name = "";
        public int Duration = 0;
        public int Remaining = 0;
        public int MaxStacks = 0;
        public int CurrentStacks = 0;

        public DispelType Dispel = DispelType.None;

        public List<Parameter> Parameters = new();
        private List<Effect> Effects = new();

        public string Tooltip = "";
    }
}
