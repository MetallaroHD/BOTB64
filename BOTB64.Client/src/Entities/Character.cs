using BOTB64.Engine;
using BOTB64.Entities.Effects;
using BOTB64.Graphics.G3D;
using BOTB64.Graphics.UI;
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

    public class Character : IReadable
    {
        public ModelInstance Model;
        public bool IsAnimating = false;
        public Vector3 VisualPosition;

        public bool Alive = false;

        // The incremental in-game id
        public int GameID = 0;
        // The database id
        public int ID = 0;
        public string Name = "";

        public Hex Position;

        public int MaxHP = 1;
        public int MaxRes = 0;
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

        public int CurrentHP = 1;
        public int CurrentResource = 0;
        public int HPRegen = 0;

        public int RemainMovement = 0;
        public int RemainAction = 0;
        public int RemainFastAction = 0;

        public int AutoAttackRange = 0;
        public float AutoAttackAP = 1f;
        public float AutoAttackSP = 0f;
        public float AutoAttackDef = 1f;
        public float AutoAttackMDef = 0f;
        public DamageType AutoAttackDamageType = DamageType.Physical;

        public ResourceType ResType = ResourceType.Mana;
        public Faction Faction = Faction.Neutral;

        public List<int> AuraIDs = new();
        public List<int> SpellIDs = new();

        // maps spells to keybinds (1-5)
        public Dictionary<int, Spell> ActiveSpells = new();
        public List<Aura> Auras = new();
        public List<Parameter> CustomParameters = new();

        public void Draw()
        {
            Model.Transform.Position = IsAnimating ? VisualPosition : HexAlgo.HexToWorld(Position);
            Model?.Draw();
        }

        public void Die()
        {
            Alive = false;
        }
    }
}
