using BOTB64.Editor.ViewModels;

namespace BOTB64.Editor.Controls
{
    public class FlagOption : ObservableObject
    {
        public string Name { get; }
        public ulong BitValue { get; }

        private bool _isChecked;
        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                if (Set(ref _isChecked, value))
                    Changed?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler Changed;

        public FlagOption(string name, ulong bitValue)
        {
            Name = name;
            BitValue = bitValue;
        }
    }
}
