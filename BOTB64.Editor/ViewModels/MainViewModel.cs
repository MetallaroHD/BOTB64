using System.Windows;
using BOTB64.Editor.IO;
using BOTB64.Editor.Models;
using Microsoft.Win32;

namespace BOTB64.Editor.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private object _currentEntity;
        private FileKind _currentKind = FileKind.Unknown;
        private string _currentPath;
        private string _statusText = "No file loaded.";

        public object CurrentEntity
        {
            get => _currentEntity;
            set => Set(ref _currentEntity, value);
        }

        public FileKind CurrentKind
        {
            get => _currentKind;
            set => Set(ref _currentKind, value);
        }

        public string CurrentPath
        {
            get => _currentPath;
            set => Set(ref _currentPath, value);
        }

        public string StatusText
        {
            get => _statusText;
            set => Set(ref _statusText, value);
        }

        public RelayCommand OpenCommand { get; }
        public RelayCommand SaveCommand { get; }
        public RelayCommand SaveAsCommand { get; }
        public RelayCommand<FileKind> NewCommand { get; }

        public MainViewModel()
        {
            OpenCommand = new RelayCommand(Open);
            SaveCommand = new RelayCommand(Save, () => CurrentEntity != null);
            SaveAsCommand = new RelayCommand(SaveAs, () => CurrentEntity != null);
            NewCommand = new RelayCommand<FileKind>(New);
        }

        private void Open()
        {
            var dlg = new OpenFileDialog
            {
                Filter = "BOTB64 files (*.b64c;*.b64s;*.b64a;*.b64t)|*.b64c;*.b64s;*.b64a;*.b64t|All files (*.*)|*.*"
            };

            if (dlg.ShowDialog() != true)
                return;

            try
            {
                var kind = FileKindDetector.Detect(dlg.FileName);

                object entity = kind switch
                {
                    FileKind.Character => CharacterIO.Read(dlg.FileName),
                    FileKind.Spell => SpellIO.Read(dlg.FileName),
                    FileKind.Aura => AuraIO.Read(dlg.FileName),
                    FileKind.TileEffect => TileEffectIO.Read(dlg.FileName),
                    _ => null
                };

                if (entity == null)
                {
                    MessageBox.Show($"Could not recognize the header of '{dlg.FileName}'.", "Unknown format",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                CurrentEntity = entity;
                CurrentKind = kind;
                CurrentPath = dlg.FileName;
                StatusText = $"Loaded {kind}: {dlg.FileName}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load file:\n{ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void New(FileKind kind)
        {
            CurrentEntity = kind switch
            {
                FileKind.Character => new CharacterModel(),
                FileKind.Spell => new SpellModel(),
                FileKind.Aura => new AuraModel(),
                FileKind.TileEffect => new TileEffectModel(),
                _ => null
            };
            CurrentKind = kind;
            CurrentPath = null;
            StatusText = $"New {kind} (unsaved)";
        }

        private void Save()
        {
            if (CurrentPath == null)
            {
                SaveAs();
                return;
            }

            WriteCurrent(CurrentPath);
        }

        private void SaveAs()
        {
            if (CurrentEntity == null)
                return;

            var dlg = new SaveFileDialog
            {
                Filter = "BOTB64 files (*.b64c;*.b64s;*.b64a;*.b64t)|*.b64c;*.b64s;*.b64a;*.b64t|All files (*.*)|*.*",
                FileName = CurrentPath ?? ("newfile" + FileKindDetector.ExtensionFor(CurrentKind))
            };

            if (dlg.ShowDialog() != true)
                return;

            CurrentPath = dlg.FileName;
            WriteCurrent(CurrentPath);
        }

        private void WriteCurrent(string path)
        {
            try
            {
                switch (CurrentKind)
                {
                    case FileKind.Character: CharacterIO.Write(path, (CharacterModel)CurrentEntity); break;
                    case FileKind.Spell: SpellIO.Write(path, (SpellModel)CurrentEntity); break;
                    case FileKind.Aura: AuraIO.Write(path, (AuraModel)CurrentEntity); break;
                    case FileKind.TileEffect: TileEffectIO.Write(path, (TileEffectModel)CurrentEntity); break;
                }
                StatusText = $"Saved {CurrentKind}: {path}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save file:\n{ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
