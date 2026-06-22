using BOTB64.Runtime;

namespace BOTB64.Entities
{
    public class TileEffect
    {
        public int ID = 0;
        public string Name = "";
        public int Duration = 0;
        public int Remaining = 0;

        public DispelType Dispel = DispelType.None;

        public List<Parameter> Parameters = new();
        public List<Effect> Effects = new();
    }
}
