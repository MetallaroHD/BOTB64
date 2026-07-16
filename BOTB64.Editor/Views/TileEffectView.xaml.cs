using System.Windows;
using System.Windows.Controls;
using BOTB64.Editor.Models;

namespace BOTB64.Editor.Views
{
    public partial class TileEffectView : UserControl
    {
        public TileEffectView()
        {
            InitializeComponent();
        }

        private TileEffectModel Fx => DataContext as TileEffectModel;

        private void AddParameter_Click(object sender, RoutedEventArgs e)
        {
            Fx?.Parameters.Add(new ParameterModel());
        }

        private void ParameterRow_RemoveRequested(object sender, ParameterModel e)
        {
            Fx?.Parameters.Remove(e);
        }

        private void AddEffect_Click(object sender, RoutedEventArgs e)
        {
            Fx?.Effects.Add(new EffectModel());
        }

        private void EffectRow_RemoveRequested(object sender, EffectModel e)
        {
            Fx?.Effects.Remove(e);
        }
    }
}
