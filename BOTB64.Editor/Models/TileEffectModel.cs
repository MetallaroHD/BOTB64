using System.Collections.ObjectModel;
using BOTB64.Editor.ViewModels;
using BOTB64.Entities;

namespace BOTB64.Editor.Models
{
    public class TileEffectModel : ObservableObject
    {
        private int _duration = 0;
        private DispelType _dispel = DispelType.None;
        private TileEffectApplicableTile _tileType = TileEffectApplicableTile.None;
        private TileEffectFlag _flags = TileEffectFlag.None;
        private TileEffectType _type = TileEffectType.None;

        public int Duration { get => _duration; set => Set(ref _duration, value); }
        public DispelType Dispel { get => _dispel; set => Set(ref _dispel, value); }
        public TileEffectApplicableTile TileType { get => _tileType; set => Set(ref _tileType, value); }
        public TileEffectFlag Flags { get => _flags; set => Set(ref _flags, value); }
        public TileEffectType Type { get => _type; set => Set(ref _type, value); }

        public ObservableCollection<ParameterModel> Parameters { get; } = new();
        public ObservableCollection<EffectModel> Effects { get; } = new();
    }
}
