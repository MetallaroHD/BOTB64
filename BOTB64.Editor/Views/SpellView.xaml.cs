using System.Windows;
using System.Windows.Controls;
using BOTB64.Editor.Models;

namespace BOTB64.Editor.Views
{
    public partial class SpellView : UserControl
    {
        public SpellView()
        {
            InitializeComponent();
        }

        private SpellModel Spell => DataContext as SpellModel;

        private void AddParameter_Click(object sender, RoutedEventArgs e)
        {
            Spell?.Parameters.Add(new ParameterModel());
        }

        private void ParameterRow_RemoveRequested(object sender, ParameterModel e)
        {
            Spell?.Parameters.Remove(e);
        }

        private void AddEffect_Click(object sender, RoutedEventArgs e)
        {
            Spell?.Effects.Add(new EffectModel());
        }

        private void EffectRow_RemoveRequested(object sender, EffectModel e)
        {
            Spell?.Effects.Remove(e);
        }
    }
}
