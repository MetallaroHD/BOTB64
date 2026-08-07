using System.IO;
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

        // Backs the Database tab (Characters/Spells/Auras/TileEffects JSON lists).
        public DatabaseViewModel Database { get; }

        public RelayCommand OpenCommand { get; }
        public RelayCommand SaveCommand { get; }
        public RelayCommand SaveAsCommand { get; }
        public RelayCommand<FileKind> NewCommand { get; }
        public RelayCommand ChooseDataRootCommand { get; }

        public MainViewModel()
        {
            OpenCommand = new RelayCommand(Open);
            SaveCommand = new RelayCommand(Save, () => CurrentEntity != null);
            SaveAsCommand = new RelayCommand(SaveAs, () => CurrentEntity != null);
            NewCommand = new RelayCommand<FileKind>(New);
            ChooseDataRootCommand = new RelayCommand(ChooseDataRoot);

            Database = new DatabaseViewModel();

            var settings = EditorSettings.Load();
            if (!string.IsNullOrEmpty(settings.DataRoot) && Directory.Exists(settings.DataRoot))
            {
                Database.DataRoot = settings.DataRoot;
                try
                {
                    Database.LoadAll();
                    StatusText = $"Loaded database from {settings.DataRoot}";
                }
                catch (Exception ex)
                {
                    StatusText = $"Failed to load database from {settings.DataRoot}: {ex.Message}";
                }
            }
        }

        private void Open()
        {
            var dlg = new OpenFileDialog
            {
                Filter = "BOTB64 files (*.b64c;*.b64s;*.b64a;*.b64t)|*.b64c;*.b64s;*.b64a;*.b64t|All files (*.*)|*.*"
            };

            if (dlg.ShowDialog() != true)
                return;

            OpenPath(dlg.FileName);
        }

        // Loads an existing file into the editor. Shared by the File > Open
        // dialog and by double-clicking a row in the Database tab.
        public void OpenPath(string path)
        {
            try
            {
                var kind = FileKindDetector.Detect(path);

                object entity = kind switch
                {
                    FileKind.Character => CharacterIO.Read(path),
                    FileKind.Spell => SpellIO.Read(path),
                    FileKind.Aura => AuraIO.Read(path),
                    FileKind.TileEffect => TileEffectIO.Read(path),
                    _ => null
                };

                if (entity == null)
                {
                    MessageBox.Show($"Could not recognize the header of '{path}'.", "Unknown format",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                CurrentEntity = entity;
                CurrentKind = kind;
                CurrentPath = path;
                StatusText = $"Loaded {kind}: {path}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load file:\n{ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Used by the Database tab: opens the file backing a database row if
        // it already exists, or offers to create a blank one at that path
        // (e.g. a row was just added and its script file doesn't exist yet).
        public void OpenOrCreate(FileKind kind, string path)
        {
            if (File.Exists(path))
            {
                OpenPath(path);
                return;
            }

            var result = MessageBox.Show(
                $"'{path}' doesn't exist yet.\n\nCreate a new blank {kind} there?",
                "Create file", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
                return;

            CurrentEntity = kind switch
            {
                FileKind.Character => new CharacterModel(),
                FileKind.Spell => new SpellModel(),
                FileKind.Aura => new AuraModel(),
                FileKind.TileEffect => new TileEffectModel(),
                _ => null
            };
            CurrentKind = kind;
            CurrentPath = path;
            StatusText = $"New {kind} (not yet saved to {path})";
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
                Directory.CreateDirectory(Path.GetDirectoryName(path)!);

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

        private void ChooseDataRoot()
        {
            // Requires .NET 8+ WPF (Microsoft.Win32.OpenFolderDialog). If your
            // project targets an older TFM, swap this for
            // System.Windows.Forms.FolderBrowserDialog instead.
            var dlg = new OpenFolderDialog
            {
                Title = "Select the BOTB64 data root (folder containing Characters\\, Spells\\, Auras\\, TileEffects\\, Graphics\\)"
            };
            if (!string.IsNullOrEmpty(Database.DataRoot))
                dlg.InitialDirectory = Database.DataRoot;

            if (dlg.ShowDialog() != true)
                return;

            Database.DataRoot = dlg.FolderName;
            EditorSettings.Save(new EditorSettings.Data { DataRoot = dlg.FolderName });

            try
            {
                Database.LoadAll();
                StatusText = $"Loaded database from {dlg.FolderName}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load database:\n{ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
