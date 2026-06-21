using System.Reflection.Metadata;

namespace BOTB64.Entities
{
    public enum DispelType
    {
        None = 0,
        Magic = 1,
        Poison = 2,
        Bleed = 3,
    }

    public class Aura
    {
        // add visuals later 

        public int ID = 0;
        public string Name = "";
        public int Duration = 0;
        public int Remaining = 0;
        public int MaxStacks = 0;
        public int CurrentStacks = 0;

        public DispelType Dispel = DispelType.None;

        public List<Parameter> Parameters = new();
        public List<Effect> Effects = new();

        public string Tooltip = "";
    }
}
