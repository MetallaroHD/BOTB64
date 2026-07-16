using System.Collections.ObjectModel;
using BOTB64.Editor.ViewModels;
using BOTB64.Engine;

namespace BOTB64.Editor.Models
{
    public class SpellModel : ObservableObject
    {
        private int _range = 0;
        private int _cooldown = 0;
        private int _charges = 0;
        private int _cost = 0;
        private float _costPct = 0f;
        private int _costHP = 0;
        private int _preparation = 0;
        private TargetingType _explicitTarget = TargetingType.Direct;
        private string _tooltip = "";

        public int Range { get => _range; set => Set(ref _range, value); }
        public int Cooldown { get => _cooldown; set => Set(ref _cooldown, value); }
        public int Charges { get => _charges; set => Set(ref _charges, value); }
        public int Cost { get => _cost; set => Set(ref _cost, value); }
        public float CostPct { get => _costPct; set => Set(ref _costPct, value); }
        public int CostHP { get => _costHP; set => Set(ref _costHP, value); }
        public int Preparation { get => _preparation; set => Set(ref _preparation, value); }
        public TargetingType ExplicitTarget { get => _explicitTarget; set => Set(ref _explicitTarget, value); }
        public string Tooltip { get => _tooltip; set => Set(ref _tooltip, value ?? ""); }

        public ObservableCollection<ParameterModel> Parameters { get; } = new();
        public ObservableCollection<EffectModel> Effects { get; } = new();
    }
}
