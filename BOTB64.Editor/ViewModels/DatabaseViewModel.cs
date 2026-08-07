using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using BOTB64.Editor.IO;
using BOTB64.Shared.DTOs;

namespace BOTB64.Editor.ViewModels
{
    // Holds the 4 JSON "database" lists shown in the Database tab.
    // Reads/writes plain files with System.Text.Json directly - deliberately
    // not going through DataFile/JsonDataFile<T>, since those are wired for
    // the shipped client (DEVELOPMENT symbol + ResourceArchive fallback) and
    // the editor always just wants loose files under a user-chosen folder.
    public class DatabaseViewModel : ObservableObject
    {
        private string _dataRoot;

        public string DataRoot
        {
            get => _dataRoot;
            set => Set(ref _dataRoot, value);
        }

        public ObservableCollection<CharacterDTO> Characters { get; } = new();
        public ObservableCollection<SpellDTO> Spells { get; } = new();
        public ObservableCollection<AuraDTO> Auras { get; } = new();
        public ObservableCollection<TileEffectDTO> TileEffects { get; } = new();

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };

        public void LoadAll()
        {
            LoadInto(EditorPaths.CharactersJson(DataRoot), Characters);
            LoadInto(EditorPaths.SpellsJson(DataRoot), Spells);
            LoadInto(EditorPaths.AurasJson(DataRoot), Auras);
            LoadInto(EditorPaths.TileEffectsJson(DataRoot), TileEffects);
        }

        private static void LoadInto<T>(string path, ObservableCollection<T> target)
        {
            target.Clear();
            if (!File.Exists(path))
                return;

            var items = JsonSerializer.Deserialize<List<T>>(File.ReadAllText(path), JsonOptions) ?? new List<T>();
            foreach (var item in items)
                target.Add(item);
        }

        public void SaveCharacters() => Save(EditorPaths.CharactersJson(DataRoot), Characters);
        public void SaveSpells() => Save(EditorPaths.SpellsJson(DataRoot), Spells);
        public void SaveAuras() => Save(EditorPaths.AurasJson(DataRoot), Auras);
        public void SaveTileEffects() => Save(EditorPaths.TileEffectsJson(DataRoot), TileEffects);

        private static void Save<T>(string path, ObservableCollection<T> items)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            File.WriteAllText(path, JsonSerializer.Serialize(items.ToList(), JsonOptions));
        }

        public int NextCharacterId() => Characters.Count == 0 ? 0 : Characters.Max(c => c.ID) + 1;
        public int NextSpellId() => Spells.Count == 0 ? 0 : Spells.Max(s => s.ID) + 1;
        public int NextAuraId() => Auras.Count == 0 ? 0 : Auras.Max(a => a.ID) + 1;
        public int NextTileEffectId() => TileEffects.Count == 0 ? 0 : TileEffects.Max(t => t.ID) + 1;
    }
}
