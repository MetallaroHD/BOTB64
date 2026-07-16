using System.Windows;
using System.Windows.Controls;
using BOTB64.Editor.Models;

namespace BOTB64.Editor.Controls
{
    public partial class EffectRow : UserControl
    {
        public static readonly DependencyProperty AllowRemoveProperty =
            DependencyProperty.Register(nameof(AllowRemove), typeof(bool), typeof(EffectRow),
                new PropertyMetadata(true, OnAllowRemoveChanged));

        public bool AllowRemove
        {
            get => (bool)GetValue(AllowRemoveProperty);
            set => SetValue(AllowRemoveProperty, value);
        }

        public event EventHandler<EffectModel> RemoveRequested;

        public EffectRow()
        {
            InitializeComponent();
        }

        private static void OnAllowRemoveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var row = (EffectRow)d;
            row.RemoveButton.Visibility = (bool)e.NewValue ? Visibility.Visible : Visibility.Collapsed;
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is EffectModel eff)
                RemoveRequested?.Invoke(this, eff);
        }
    }
}
