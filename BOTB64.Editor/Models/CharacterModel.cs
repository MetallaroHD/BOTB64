using System.Collections.ObjectModel;
using BOTB64.Editor.ViewModels;
using BOTB64.Entities;

namespace BOTB64.Editor.Models
{
    public class CharacterModel : ObservableObject
    {
        private int _maxHP = 50;
        private int _maxRes = 25;
        private int _hpRegen = 0;
        private int _resRegen = 0;
        private int _startRes = 15;
        private int _attackPower = 30;
        private int _spellPower = 0;
        private int _defense = 5;
        private int _magicDefense = 0;
        private int _haste = 4;
        private int _speed = 5;
        private float _armorPen = 0f;
        private float _spellPen = 0f;
        private float _crit = 0f;
        private float _lifeSteal = 0f;
        private float _spellVamp = 0f;
        private int _autoAttackRange = 4;
        private float _autoAttackAP = 1f;
        private float _autoAttackSP = 0f;
        private ResourceType _resType = ResourceType.Mana;
        private EffectModel _autoAttackEffect = new();

        public int MaxHP { get => _maxHP; set => Set(ref _maxHP, value); }
        public int MaxRes { get => _maxRes; set => Set(ref _maxRes, value); }
        public int HPRegen { get => _hpRegen; set => Set(ref _hpRegen, value); }
        public int ResRegen { get => _resRegen; set => Set(ref _resRegen, value); }
        public int StartRes { get => _startRes; set => Set(ref _startRes, value); }
        public int AttackPower { get => _attackPower; set => Set(ref _attackPower, value); }
        public int SpellPower { get => _spellPower; set => Set(ref _spellPower, value); }
        public int Defense { get => _defense; set => Set(ref _defense, value); }
        public int MagicDefense { get => _magicDefense; set => Set(ref _magicDefense, value); }
        public int Haste { get => _haste; set => Set(ref _haste, value); }
        public int Speed { get => _speed; set => Set(ref _speed, value); }
        public float ArmorPen { get => _armorPen; set => Set(ref _armorPen, value); }
        public float SpellPen { get => _spellPen; set => Set(ref _spellPen, value); }
        public float Crit { get => _crit; set => Set(ref _crit, value); }
        public float LifeSteal { get => _lifeSteal; set => Set(ref _lifeSteal, value); }
        public float SpellVamp { get => _spellVamp; set => Set(ref _spellVamp, value); }
        public int AutoAttackRange { get => _autoAttackRange; set => Set(ref _autoAttackRange, value); }
        public float AutoAttackAP { get => _autoAttackAP; set => Set(ref _autoAttackAP, value); }
        public float AutoAttackSP { get => _autoAttackSP; set => Set(ref _autoAttackSP, value); }
        public ResourceType ResType { get => _resType; set => Set(ref _resType, value); }
        public EffectModel AutoAttackEffect { get => _autoAttackEffect; set => Set(ref _autoAttackEffect, value); }

        public ObservableCollection<IntEntry> PermanentAuras { get; } = new();
        public ObservableCollection<KeybindEntry> SpellLoadout { get; } = new();
    }
}
