using BOTB64.Graphics.G3D;
using BOTB64.Runtime;
using System.Numerics;

namespace BOTB64.Entities
{
    public enum Faction
    {
        Neutral = 0,
        BlueTeam = 1,
        RedTeam = 2,
        Hostile = 3,
    }

    public enum ResourceType
    {
        Mana = 0,
    }

    public class Character
    {
        public ModelInstance Model;

        public int ID = 0;
        public string Name = "";

        public int MaxHP = 0;
        public int MaxRes = 0;
        public ResourceType ResType = ResourceType.Mana;
        public int ResRegen = 0;
        public int StartRes = 0;
        public int AttackPower = 0;
        public int SpellPower = 0;
        public int Defense = 0;
        public int MagicDefense = 0;
        public int Haste = 0;
        public int Speed = 0;

        public float Crit = 0f;
        public float LifeSteal = 0f;

        public int CurrentHP = 0;
        public int CurrentResource = 0;
        public int HPRegen = 0;

        public int RemainMovement = 0;
        public int RemainAction = 0;
        public int RemainFastAction = 0;

        public Faction Faction = Faction.Neutral;

        Dictionary<int, Spell> ActiveSpells = new();
        List<Aura> Auras = new();
        List<Parameter> CustomParameters = new();

        public void Draw()
        {
            Model?.Draw();
        }
    }
}
