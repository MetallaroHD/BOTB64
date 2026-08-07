using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BOTB64.Editor.IO;
using BOTB64.Editor.ViewModels;
using BOTB64.Shared.DTOs;
using Microsoft.Win32;

namespace BOTB64.Editor.Views
{
    public partial class DatabaseView : UserControl
    {
        // Raised when the user double-clicks a row and wants its script
        // opened (or created) in the main editor tab. MainWindow subscribes
        // and forwards to MainViewModel.OpenOrCreate.
        public event Action<FileKind, string> OpenScriptRequested;

        private DatabaseViewModel Db => DataContext as DatabaseViewModel;

        public DatabaseView()
        {
            InitializeComponent();
        }

        private bool RequireDataRoot()
        {
            if (Db != null && !string.IsNullOrEmpty(Db.DataRoot))
                return true;

            MessageBox.Show("Set a data root first (File > Data Root...).", "No data root",
                MessageBoxButton.OK, MessageBoxImage.Information);
            return false;
        }

        private static void BrowseInto(string initialDir, string filter, Action<string> applyNameWithoutExtension, DataGrid grid)
        {
            try
            {
                Directory.CreateDirectory(initialDir);
            }
            catch
            {
                // If the folder can't be created (e.g. invalid Subdir so far),
                // still let the dialog open at whatever default it falls back to.
            }

            var dlg = new OpenFileDialog { InitialDirectory = initialDir, Filter = filter };
            if (dlg.ShowDialog() != true)
                return;

            applyNameWithoutExtension(Path.GetFileNameWithoutExtension(dlg.FileName));
            grid.Items.Refresh();
        }

        private void SaveWithFeedback(Action save, string label)
        {
            try
            {
                save();
                MessageBox.Show($"Saved {label}.", "Saved", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save {label}:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ============================== CHARACTERS ==============================

        private void AddCharacter_Click(object sender, RoutedEventArgs e)
        {
            if (Db == null) return;
            Db.Characters.Add(new CharacterDTO
            {
                ID = Db.NextCharacterId(),
                Enabled = true,
                Name = "New Character",
                Subdir = "NewCharacter",
                ScriptURI = "character",
                ModelURI = "",
                IconURI = "dummy"
            });
        }

        private void RemoveCharacter_Click(object sender, RoutedEventArgs e)
        {
            if (Db != null && CharactersGrid.SelectedItem is CharacterDTO row)
                Db.Characters.Remove(row);
        }

        private void SaveCharacters_Click(object sender, RoutedEventArgs e) =>
            SaveWithFeedback(() => Db?.SaveCharacters(), "characters.json");

        private void BrowseCharacterScript_Click(object sender, RoutedEventArgs e)
        {
            if (!RequireDataRoot() || (sender as FrameworkElement)?.DataContext is not CharacterDTO row) return;
            BrowseInto(EditorPaths.CharacterSubdirPath(Db.DataRoot, row.Subdir),
                "Character script (*.b64c)|*.b64c|All files (*.*)|*.*",
                v => row.ScriptURI = v, CharactersGrid);
        }

        private void BrowseCharacterModel_Click(object sender, RoutedEventArgs e)
        {
            if (!RequireDataRoot() || (sender as FrameworkElement)?.DataContext is not CharacterDTO row) return;
            BrowseInto(EditorPaths.CharacterSubdirPath(Db.DataRoot, row.Subdir),
                "Model files (*.glb)|*.glb|All files (*.*)|*.*",
                v => row.ModelURI = v, CharactersGrid);
        }

        private void BrowseCharacterIcon_Click(object sender, RoutedEventArgs e)
        {
            if (!RequireDataRoot() || (sender as FrameworkElement)?.DataContext is not CharacterDTO row) return;
            BrowseInto(EditorPaths.CharacterSubdirPath(Db.DataRoot, row.Subdir),
                "Image files (*.png)|*.png|All files (*.*)|*.*",
                v => row.IconURI = v, CharactersGrid);
        }

        private void CharactersGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!RequireDataRoot() || CharactersGrid.SelectedItem is not CharacterDTO row) return;
            OpenScriptRequested?.Invoke(FileKind.Character, EditorPaths.CharacterScriptPath(Db.DataRoot, row));
        }

        // ================================ SPELLS ================================

        private void AddSpell_Click(object sender, RoutedEventArgs e)
        {
            if (Db == null) return;
            Db.Spells.Add(new SpellDTO
            {
                ID = Db.NextSpellId(),
                Name = "New Spell",
                ScriptURI = "spell",
                IconURI = "dummy",
                AnimationURI = ""
            });
        }

        private void RemoveSpell_Click(object sender, RoutedEventArgs e)
        {
            if (Db != null && SpellsGrid.SelectedItem is SpellDTO row)
                Db.Spells.Remove(row);
        }

        private void SaveSpells_Click(object sender, RoutedEventArgs e) =>
            SaveWithFeedback(() => Db?.SaveSpells(), "spells.json");

        private void BrowseSpellScript_Click(object sender, RoutedEventArgs e)
        {
            if (!RequireDataRoot() || (sender as FrameworkElement)?.DataContext is not SpellDTO row) return;
            BrowseInto(EditorPaths.SpellsDir(Db.DataRoot),
                "Spell script (*.b64s)|*.b64s|All files (*.*)|*.*",
                v => row.ScriptURI = v, SpellsGrid);
        }

        private void BrowseSpellIcon_Click(object sender, RoutedEventArgs e)
        {
            if (!RequireDataRoot() || (sender as FrameworkElement)?.DataContext is not SpellDTO row) return;
            BrowseInto(EditorPaths.GraphicsIconsDir(Db.DataRoot),
                "Image files (*.png)|*.png|All files (*.*)|*.*",
                v => row.IconURI = v, SpellsGrid);
        }

        private void BrowseSpellAnimation_Click(object sender, RoutedEventArgs e)
        {
            if (!RequireDataRoot() || (sender as FrameworkElement)?.DataContext is not SpellDTO row) return;
            BrowseInto(EditorPaths.GraphicsAnimationsDir(Db.DataRoot),
                "All files (*.*)|*.*",
                v => row.AnimationURI = v, SpellsGrid);
        }

        private void SpellsGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!RequireDataRoot() || SpellsGrid.SelectedItem is not SpellDTO row) return;
            OpenScriptRequested?.Invoke(FileKind.Spell, EditorPaths.SpellScriptPath(Db.DataRoot, row));
        }

        // ================================= AURAS =================================

        private void AddAura_Click(object sender, RoutedEventArgs e)
        {
            if (Db == null) return;
            Db.Auras.Add(new AuraDTO
            {
                ID = Db.NextAuraId(),
                Name = "New Aura",
                ScriptURI = "aura",
                IconURI = "dummy",
                AnimationURI = ""
            });
        }

        private void RemoveAura_Click(object sender, RoutedEventArgs e)
        {
            if (Db != null && AurasGrid.SelectedItem is AuraDTO row)
                Db.Auras.Remove(row);
        }

        private void SaveAuras_Click(object sender, RoutedEventArgs e) =>
            SaveWithFeedback(() => Db?.SaveAuras(), "auras.json");

        private void BrowseAuraScript_Click(object sender, RoutedEventArgs e)
        {
            if (!RequireDataRoot() || (sender as FrameworkElement)?.DataContext is not AuraDTO row) return;
            BrowseInto(EditorPaths.AurasDir(Db.DataRoot),
                "Aura script (*.b64a)|*.b64a|All files (*.*)|*.*",
                v => row.ScriptURI = v, AurasGrid);
        }

        private void BrowseAuraIcon_Click(object sender, RoutedEventArgs e)
        {
            if (!RequireDataRoot() || (sender as FrameworkElement)?.DataContext is not AuraDTO row) return;
            BrowseInto(EditorPaths.GraphicsIconsDir(Db.DataRoot),
                "Image files (*.png)|*.png|All files (*.*)|*.*",
                v => row.IconURI = v, AurasGrid);
        }

        private void BrowseAuraAnimation_Click(object sender, RoutedEventArgs e)
        {
            if (!RequireDataRoot() || (sender as FrameworkElement)?.DataContext is not AuraDTO row) return;
            BrowseInto(EditorPaths.GraphicsAnimationsDir(Db.DataRoot),
                "All files (*.*)|*.*",
                v => row.AnimationURI = v, AurasGrid);
        }

        private void AurasGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!RequireDataRoot() || AurasGrid.SelectedItem is not AuraDTO row) return;
            OpenScriptRequested?.Invoke(FileKind.Aura, EditorPaths.AuraScriptPath(Db.DataRoot, row));
        }

        // ============================== TILE EFFECTS ==============================

        private void AddTileEffect_Click(object sender, RoutedEventArgs e)
        {
            if (Db == null) return;
            Db.TileEffects.Add(new TileEffectDTO
            {
                ID = Db.NextTileEffectId(),
                Name = "New Tile Effect",
                ScriptURI = "tileeffect",
                ImageURI = "",
                AnimationURI = "",
                ModelURI = ""
            });
        }

        private void RemoveTileEffect_Click(object sender, RoutedEventArgs e)
        {
            if (Db != null && TileEffectsGrid.SelectedItem is TileEffectDTO row)
                Db.TileEffects.Remove(row);
        }

        private void SaveTileEffects_Click(object sender, RoutedEventArgs e) =>
            SaveWithFeedback(() => Db?.SaveTileEffects(), "tileEffects.json");

        private void BrowseTileEffectScript_Click(object sender, RoutedEventArgs e)
        {
            if (!RequireDataRoot() || (sender as FrameworkElement)?.DataContext is not TileEffectDTO row) return;
            BrowseInto(EditorPaths.TileEffectsDir(Db.DataRoot),
                "Tile effect script (*.b64t)|*.b64t|All files (*.*)|*.*",
                v => row.ScriptURI = v, TileEffectsGrid);
        }

        private void BrowseTileEffectImage_Click(object sender, RoutedEventArgs e)
        {
            if (!RequireDataRoot() || (sender as FrameworkElement)?.DataContext is not TileEffectDTO row) return;
            BrowseInto(EditorPaths.GraphicsIconsDir(Db.DataRoot),
                "Image files (*.png)|*.png|All files (*.*)|*.*",
                v => row.ImageURI = v, TileEffectsGrid);
        }

        private void BrowseTileEffectModel_Click(object sender, RoutedEventArgs e)
        {
            if (!RequireDataRoot() || (sender as FrameworkElement)?.DataContext is not TileEffectDTO row) return;
            BrowseInto(EditorPaths.GraphicsModelsDir(Db.DataRoot),
                "Model files (*.glb)|*.glb|All files (*.*)|*.*",
                v => row.ModelURI = v, TileEffectsGrid);
        }

        private void BrowseTileEffectAnimation_Click(object sender, RoutedEventArgs e)
        {
            if (!RequireDataRoot() || (sender as FrameworkElement)?.DataContext is not TileEffectDTO row) return;
            BrowseInto(EditorPaths.GraphicsAnimationsDir(Db.DataRoot),
                "All files (*.*)|*.*",
                v => row.AnimationURI = v, TileEffectsGrid);
        }

        private void TileEffectsGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!RequireDataRoot() || TileEffectsGrid.SelectedItem is not TileEffectDTO row) return;
            OpenScriptRequested?.Invoke(FileKind.TileEffect, EditorPaths.TileEffectScriptPath(Db.DataRoot, row));
        }
    }
}
