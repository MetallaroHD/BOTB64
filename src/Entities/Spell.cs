using BOTB64.Runtime;

namespace BOTB64.Entities
{
    public class Spell
    {
        //add visuals and sound later

        public int ID = 0;
        public string Name = "";

        public int Range = 0;
        public int Cooldown = 0;
        public int Charges = 0;
        public int Cost = 0;
        public float CostPct = 0;
        public int CostHP = 0;
        public int Preparation = 0;

        public int CurrentCD = 0;

        public List<Parameter> Parameters = new();
        public List<Effect> Effects = new();

        public string Tooltip = "";
    }
}
