using System;
using System.IO;
using System.Text.Json;

namespace BOTB64.Editor.IO
{
    // Tiny persisted settings file for the editor itself (not game data) -
    // currently just remembers the last-used data root so you don't have to
    // re-pick the folder every launch.
    public static class EditorSettings
    {
        private static string SettingsPath =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "BOTB64Editor", "settings.json");

        public class Data
        {
            public string DataRoot { get; set; }
        }

        public static Data Load()
        {
            try
            {
                if (File.Exists(SettingsPath))
                    return JsonSerializer.Deserialize<Data>(File.ReadAllText(SettingsPath)) ?? new Data();
            }
            catch
            {
                // Corrupt or unreadable settings file - just fall back to defaults.
            }
            return new Data();
        }

        public static void Save(Data data)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(SettingsPath)!);
                File.WriteAllText(SettingsPath, JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true }));
            }
            catch
            {
                // Not critical if this fails - worst case you re-pick the folder next launch.
            }
        }
    }
}
