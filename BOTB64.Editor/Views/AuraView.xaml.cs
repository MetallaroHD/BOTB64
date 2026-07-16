using System.Windows;
using System.Windows.Controls;
using BOTB64.Editor.Models;

namespace BOTB64.Editor.Views
{
    public partial class AuraView : UserControl
    {
        public AuraView()
        {
            InitializeComponent();
        }

        private AuraModel Aura => DataContext as AuraModel;

        private void AddParameter_Click(object sender, RoutedEventArgs e)
        {
            Aura?.Parameters.Add(new ParameterModel());
        }

        private void ParameterRow_RemoveRequested(object sender, ParameterModel e)
        {
            Aura?.Parameters.Remove(e);
        }

        private void AddEffect_Click(object sender, RoutedEventArgs e)
        {
            Aura?.Effects.Add(new EffectModel());
        }

        private void EffectRow_RemoveRequested(object sender, EffectModel e)
        {
            Aura?.Effects.Remove(e);
        }
    }
}
