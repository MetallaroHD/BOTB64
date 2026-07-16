using System.Collections.ObjectModel;
using BOTB64.Editor.ViewModels;
using BOTB64.Entities;

namespace BOTB64.Editor.Models
{
    public class AuraModel : ObservableObject
    {
        private int _duration = 0;
        private int _maxStacks = 0;
        private DispelType _dispel = DispelType.None;
        private string _tooltip = "";

        public int Duration { get => _duration; set => Set(ref _duration, value); }
        public int MaxStacks { get => _maxStacks; set => Set(ref _maxStacks, value); }
        public DispelType Dispel { get => _dispel; set => Set(ref _dispel, value); }
        public string Tooltip { get => _tooltip; set => Set(ref _tooltip, value ?? ""); }

        public ObservableCollection<ParameterModel> Parameters { get; } = new();
        public ObservableCollection<EffectModel> Effects { get; } = new();
    }
}
