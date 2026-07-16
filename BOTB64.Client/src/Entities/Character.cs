using BOTB64.Engine;
using BOTB64.Graphics.G3D;
using BOTB64.Runtime;
using System.Numerics;
using RL = Raylib_cs;

namespace BOTB64.Entities
{
    public enum Faction
    {
        Neutral = 0,
        BlueTeam = 1,
        RedTeam = 2,
        Hostile = 3,
        Spectator = 4,
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

        // --- Base data (can still change during game) --- //
        // The database id
        public int ID = 0;
        public string Name = "";
        public int MaxHP = 50;
        public int MaxRes = 25;
        public int HPRegen = 0;
        public int ResRegen = 0;
        public int StartRes = 15;
        public int AttackPower = 30;
        public int SpellPower = 0;
        public int Defense = 5;
        public int MagicDefense = 0;
        public int Haste = 4;
        public int Speed = 5;
        public float ArmorPen = 0f; //these are all 0-1
        public float SpellPen = 0f;
        public float Crit = 0f;
        public float LifeSteal = 0f;
        public float SpellVamp = 0f;

        public Effect AutoAttackEffect = new();
        public int AutoAttackRange = 4;
        public float AutoAttackAP = 1f;
        public float AutoAttackSP = 0f;
        public ResourceType ResType = ResourceType.Mana;

        // Ids of auras that are applied at game start
        public List<int> PermanentAuras = new();
        // Starting spells
        public Dictionary<int, int> SpellLoadout = new();

        // --- Volatile data --- //
        // The incremental in-game id
        public int GameID = 0;
        // The id of the controlling player
        public int OwnerID = -1;

        public Hex Position;
        public Hex Direction = new(1, 0);

        public int CurrentHP = 50;
        public int CurrentResource = 15;

        public bool HasMovedThisTurn = false;
        public int RemainMovement = 5;
        public int RemainAction = 1;
        public int RemainFastAction = 1;
        public Faction Faction = Faction.Neutral;

        // maps spells to keybinds (1-5)
        public Dictionary<int, Spell> ActiveSpells = new();
        public List<Aura> CurrentAuras = new();
        public List<Parameter> CustomParameters = new();

        public void Draw()
        {
            int idx = HexAlgo.DirectionIndex(Direction);
            Model.Transform.RotationAngle = idx * 60f;
            Model.Transform.Position = IsAnimating ? VisualPosition : HexAlgo.HexToWorld(Position);
            Model?.Draw();
        }

        public void Assign(int newOwner)
        {
            OwnerID = newOwner;
        }

        public void Die()
        {
            Alive = false;
            Logger.Log(Name + " dies!");
        }

        public RL.Color ResourceColor()
        {
            switch (ResType)
            {
                case ResourceType.Mana:
                    return RL.Color.Blue;
                default:
                    return RL.Color.Blue;
            }
        }
    }
}
