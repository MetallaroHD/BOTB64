using BOTB64.Editor.ViewModels;

namespace BOTB64.Editor.Models
{
    // Editor-friendly stand-in for a single entry of Character.SpellLoadout
    // (Dictionary<int,int> keybind -> spellId). Kept as a flat list of rows
    // so it can bind to an ItemsControl.
    public class KeybindEntry : ObservableObject
    {
        private int _keybind;
        private int _spellId;

        public int Keybind
        {
            get => _keybind;
            set => Set(ref _keybind, value);
        }

        public int SpellId
        {
            get => _spellId;
            set => Set(ref _spellId, value);
        }
    }
}
