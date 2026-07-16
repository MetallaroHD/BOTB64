using System.Collections.ObjectModel;
using System.Globalization;
using BOTB64.Editor.Models;
using BOTB64.Runtime;

namespace BOTB64.Editor.IO
{
    // Re-implementation (for the editor) of the shared line-scanning helpers
    // found in BOTBDatafile<T> and ScriptDataFile<T>. Operates on editor
    // models instead of the game's runtime entities.
    public static class BotbParsing
    {
        public static int SkipBlankAndComments(string[] lines, int i)
        {
            while (i < lines.Length)
            {
                string t = lines[i].Trim();
                if (t.Length > 0 && !t.StartsWith("#", StringComparison.Ordinal))
                    break;
                i++;
            }
            return i;
        }

        public static (string Tag, string Value) SplitTag(string line)
        {
            int spaceIdx = line.IndexOf(' ');
            if (spaceIdx < 0)
                return (line.TrimStart(':'), "");
            return (line[..spaceIdx].TrimStart(':'), line[(spaceIdx + 1)..]);
        }

        public static string ParseQuotedOrRaw(string rest)
        {
            rest = rest.Trim();
            if (rest.Length >= 2 && rest.StartsWith('"') && rest.EndsWith('"'))
                return rest[1..^1];
            return rest;
        }

        public static int ParseParameters(string[] lines, int i, ObservableCollection<ParameterModel> target)
        {
            while (i < lines.Length)
            {
                string trimmed = lines[i].Trim();

                if (trimmed.Length == 0 || trimmed.StartsWith("#", StringComparison.Ordinal))
                {
                    i++;
                    continue;
                }

                if (trimmed.StartsWith(":", StringComparison.Ordinal))
                    break;

                string[] parts = trimmed.Split(' ', 3, StringSplitOptions.RemoveEmptyEntries);
                string name = parts[0];
                var type = (ParameterType)int.Parse(parts[1], CultureInfo.InvariantCulture);
                string raw = parts.Length > 2 ? parts[2] : "";
                target.Add(ParameterModel.FromRaw(name, type, raw));
                i++;
            }
            return i;
        }

        public static int ParseEffects(string[] lines, int i, ObservableCollection<EffectModel> target)
        {
            while (i < lines.Length)
            {
                string trimmed = lines[i].Trim();

                if (trimmed.Length == 0 || trimmed.StartsWith("#", StringComparison.Ordinal))
                {
                    i++;
                    continue;
                }

                if (trimmed.StartsWith(":", StringComparison.Ordinal))
                    break;

                string[] parts = trimmed.Split('|', StringSplitOptions.TrimEntries);
                if (parts.Length != 5)
                    throw new FormatException($"Malformed effect line: '{trimmed}'");

                target.Add(EffectModel.FromParts(
                    parts[0],
                    int.Parse(parts[1], CultureInfo.InvariantCulture),
                    int.Parse(parts[2], CultureInfo.InvariantCulture),
                    int.Parse(parts[3], CultureInfo.InvariantCulture),
                    int.Parse(parts[4], CultureInfo.InvariantCulture)));

                i++;
            }
            return i;
        }

        public static int ParseIntList(string[] lines, int i, ObservableCollection<IntEntry> target)
        {
            while (i < lines.Length)
            {
                string trimmed = lines[i].Trim();

                if (trimmed.Length == 0 || trimmed.StartsWith("#", StringComparison.Ordinal))
                {
                    i++;
                    continue;
                }

                if (trimmed.StartsWith(":", StringComparison.Ordinal))
                    break;

                target.Add(new IntEntry(int.Parse(trimmed, CultureInfo.InvariantCulture)));
                i++;
            }
            return i;
        }

        public static int ParseSpellLoadout(string[] lines, int i, ObservableCollection<KeybindEntry> target)
        {
            while (i < lines.Length)
            {
                string trimmed = lines[i].Trim();

                if (trimmed.Length == 0 || trimmed.StartsWith("#", StringComparison.Ordinal))
                {
                    i++;
                    continue;
                }

                if (trimmed.StartsWith(":", StringComparison.Ordinal))
                    break;

                string[] parts = trimmed.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                target.Add(new KeybindEntry
                {
                    Keybind = int.Parse(parts[0], CultureInfo.InvariantCulture),
                    SpellId = int.Parse(parts[1], CultureInfo.InvariantCulture)
                });
                i++;
            }
            return i;
        }

        // Character's autoattack effect. The original CharacterDataFile.Read
        // looks for a ":AUTOATTACK" tag, but the sample .b64c file uses
        // ":EFFECTS" for the same section - both are accepted here on read.
        public static int ParseAutoAttack(string[] lines, int i, CharacterModel c)
        {
            while (i < lines.Length)
            {
                string trimmed = lines[i].Trim();

                if (trimmed.Length == 0 || trimmed.StartsWith("#", StringComparison.Ordinal))
                {
                    i++;
                    continue;
                }

                if (trimmed.StartsWith(":", StringComparison.Ordinal))
                    break;

                string[] parts = trimmed.Split('|', StringSplitOptions.TrimEntries);
                if (parts.Length != 5)
                    throw new FormatException($"Malformed autoattack effect line: '{trimmed}'");

                c.AutoAttackEffect = EffectModel.FromParts(
                    parts[0],
                    int.Parse(parts[1], CultureInfo.InvariantCulture),
                    int.Parse(parts[2], CultureInfo.InvariantCulture),
                    int.Parse(parts[3], CultureInfo.InvariantCulture),
                    int.Parse(parts[4], CultureInfo.InvariantCulture));

                i++;
                // only one autoattack effect expected; keep consuming/ignoring extras if present
            }
            return i;
        }
    }
}
