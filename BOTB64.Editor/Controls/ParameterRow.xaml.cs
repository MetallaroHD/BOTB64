using System.Windows;
using System.Windows.Controls;
using BOTB64.Editor.Models;

namespace BOTB64.Editor.Controls
{
    public partial class ParameterRow : UserControl
    {
        public event EventHandler<ParameterModel> RemoveRequested;

        public ParameterRow()
        {
            InitializeComponent();
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ParameterModel p)
                RemoveRequested?.Invoke(this, p);
        }
    }
}
