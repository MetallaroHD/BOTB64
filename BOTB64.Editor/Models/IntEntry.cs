using BOTB64.Editor.ViewModels;

namespace BOTB64.Editor.Models
{
    public class IntEntry : ObservableObject
    {
        private int _value;

        public int Value
        {
            get => _value;
            set => Set(ref _value, value);
        }

        public IntEntry() { }
        public IntEntry(int value) => _value = value;
    }
}
