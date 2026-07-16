using BOTB64.Entities;
using BOTB64.Shared.Files;
using System.Globalization;

namespace BOTB64.Runtime
{
    public abstract class ScriptDataFile<T> : BOTBDatafile<T> where T : ExecutableBase, IReadable, new()
    {
        protected static int ParseParameters(string[] lines, int i, List<Parameter> target)
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

                target.Add(Parameter.Create(name, type, raw));
                i++;
            }
            return i;
        }

        protected static int ParseEffects(string[] lines, int i, ExecutableBase owner)
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
                    throw new FormatException($"Malformed :EFFECTS line: '{trimmed}'");

                var effect = new Effect
                {
                    Script = parts[0],
                    Trigger = (EffectTrigger)int.Parse(parts[1], CultureInfo.InvariantCulture),
                    Source = (EffectSourceType)int.Parse(parts[2], CultureInfo.InvariantCulture),
                    Type = (EffectDamageType)int.Parse(parts[3], CultureInfo.InvariantCulture),
                    Scaling = (EffectDamageScaling)int.Parse(parts[4], CultureInfo.InvariantCulture)
                };

                owner.AddEffect(effect);
                i++;
            }
            return i;
        }

        protected static string ParseQuotedOrRaw(string rest)
        {
            rest = rest.Trim();
            if (rest.Length >= 2 && rest.StartsWith('"') && rest.EndsWith('"'))
                return rest[1..^1];
            return rest;
        }

        protected static (string Tag, string Value) SplitTag(string line)
        {
            int spaceIdx = line.IndexOf(' ');
            if (spaceIdx < 0)
                return (line.TrimStart(':'), "");
            return (line[..spaceIdx].TrimStart(':'), line[(spaceIdx + 1)..]);
        }
    }
}