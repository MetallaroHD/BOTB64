using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace BOTB64.Editor.Controls
{
    public partial class FlagsPicker : UserControl
    {
        public static readonly DependencyProperty EnumTypeProperty =
            DependencyProperty.Register(nameof(EnumType), typeof(Type), typeof(FlagsPicker),
                new PropertyMetadata(null, OnEnumTypeChanged));

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(Enum), typeof(FlagsPicker),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValueChanged));

        public static readonly DependencyProperty SummaryTextProperty =
            DependencyProperty.Register(nameof(SummaryText), typeof(string), typeof(FlagsPicker),
                new PropertyMetadata("(none)"));

        public Type EnumType
        {
            get => (Type)GetValue(EnumTypeProperty);
            set => SetValue(EnumTypeProperty, value);
        }

        public Enum Value
        {
            get => (Enum)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public string SummaryText
        {
            get => (string)GetValue(SummaryTextProperty);
            set => SetValue(SummaryTextProperty, value);
        }

        public ObservableCollection<FlagOption> Options { get; } = new();

        private bool _suppress;

        public FlagsPicker()
        {
            InitializeComponent();
        }

        private static void OnEnumTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((FlagsPicker)d).RebuildOptions();
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((FlagsPicker)d).SyncOptionsFromValue();
        }

        private void RebuildOptions()
        {
            foreach (var opt in Options)
                opt.Changed -= Option_Changed;
            Options.Clear();

            if (EnumType == null)
                return;

            foreach (var val in Enum.GetValues(EnumType))
            {
                ulong bit = Convert.ToUInt64(val);
                if (bit == 0)
                    continue; // the zero value ("None"/"Direct") is implied when nothing is checked

                var opt = new FlagOption(val.ToString(), bit);
                opt.Changed += Option_Changed;
                Options.Add(opt);
            }

            SyncOptionsFromValue();
        }

        private void SyncOptionsFromValue()
        {
            if (EnumType == null)
                return;

            ulong current = Value == null ? 0 : Convert.ToUInt64(Value);

            _suppress = true;
            foreach (var o in Options)
                o.IsChecked = (current & o.BitValue) == o.BitValue;
            _suppress = false;

            UpdateSummary();
        }

        private void Option_Changed(object sender, EventArgs e)
        {
            if (_suppress || EnumType == null)
                return;

            ulong combined = Options.Where(o => o.IsChecked).Aggregate(0UL, (acc, o) => acc | o.BitValue);
            Value = (Enum)Enum.ToObject(EnumType, combined);
            UpdateSummary();
        }

        private void UpdateSummary()
        {
            var names = Options.Where(o => o.IsChecked).Select(o => o.Name).ToList();
            SummaryText = names.Count == 0 ? "(none)" : string.Join(", ", names);
        }
    }
}
