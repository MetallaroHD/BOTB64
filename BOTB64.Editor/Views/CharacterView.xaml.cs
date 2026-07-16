using System.Windows;
using System.Windows.Controls;
using BOTB64.Editor.Models;

namespace BOTB64.Editor.Views
{
    public partial class CharacterView : UserControl
    {
        public CharacterView()
        {
            InitializeComponent();
        }

        private CharacterModel Char => DataContext as CharacterModel;

        private void AddAura_Click(object sender, RoutedEventArgs e)
        {
            Char?.PermanentAuras.Add(new IntEntry(0));
        }

        private void RemoveAura_Click(object sender, RoutedEventArgs e)
        {
            if (Char == null) return;
            if ((sender as FrameworkElement)?.DataContext is IntEntry entry)
                Char.PermanentAuras.Remove(entry);
        }

        private void AddSpell_Click(object sender, RoutedEventArgs e)
        {
            Char?.SpellLoadout.Add(new KeybindEntry { Keybind = Char.SpellLoadout.Count + 1, SpellId = 0 });
        }

        private void RemoveSpell_Click(object sender, RoutedEventArgs e)
        {
            if (Char == null) return;
            if ((sender as FrameworkElement)?.DataContext is KeybindEntry entry)
                Char.SpellLoadout.Remove(entry);
        }
    }
}
