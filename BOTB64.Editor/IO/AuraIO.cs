using System.Globalization;
using System.Text;
using BOTB64.Editor.Models;
using BOTB64.Entities;
using System.IO;

namespace BOTB64.Editor.IO
{
    public static class AuraIO
    {
        public static AuraModel Read(string path)
        {
            string[] lines = File.ReadAllLines(path);
            int i = BotbParsing.SkipBlankAndComments(lines, 0);

            if (i >= lines.Length || !lines[i].Trim().StartsWith(FileKindDetector.HeaderAura, StringComparison.Ordinal))
                throw new FormatException($"'{path}' is missing a valid :BOTB64AURA header.");
            i++;

            var aura = new AuraModel();

            while (i < lines.Length)
            {
                i = BotbParsing.SkipBlankAndComments(lines, i);
                if (i >= lines.Length) break;

                string line = lines[i].Trim();

                if (line.StartsWith(":PARAMETERS", StringComparison.Ordinal))
                {
                    i++;
                    i = BotbParsing.ParseParameters(lines, i, aura.Parameters);
                }
                else if (line.StartsWith(":EFFECTS", StringComparison.Ordinal))
                {
                    i++;
                    i = BotbParsing.ParseEffects(lines, i, aura.Effects);
                }
                else if (line.StartsWith(":", StringComparison.Ordinal))
                {
                    var (tag, rest) = BotbParsing.SplitTag(line);
                    ApplyScalar(aura, tag, rest);
                    i++;
                }
                else
                {
                    i++;
                }
            }

            return aura;
        }

        private static void ApplyScalar(AuraModel aura, string tag, string rest)
        {
            switch (tag)
            {
                case "DURATION": aura.Duration = int.Parse(rest, CultureInfo.InvariantCulture); break;
                case "MAXSTACKS": aura.MaxStacks = int.Parse(rest, CultureInfo.InvariantCulture); break;
                case "DISPEL": aura.Dispel = (DispelType)int.Parse(rest, CultureInfo.InvariantCulture); break;
                case "TOOLTIP": aura.Tooltip = BotbParsing.ParseQuotedOrRaw(rest); break;
            }
        }

        public static void Write(string path, AuraModel a)
        {
            var sb = new StringBuilder();
            var inv = CultureInfo.InvariantCulture;

            sb.AppendLine(FileKindDetector.HeaderAura);
            sb.AppendLine($":DURATION {a.Duration.ToString(inv)}");
            sb.AppendLine($":MAXSTACKS {a.MaxStacks.ToString(inv)}");
            sb.AppendLine($":DISPEL {((int)a.Dispel).ToString(inv)}");

            if (a.Parameters.Count > 0)
            {
                sb.AppendLine(":PARAMETERS");
                foreach (var p in a.Parameters)
                    sb.AppendLine($"{p.Name} {((int)p.Type).ToString(inv)} {p.ToRaw()}");
            }

            sb.AppendLine($":TOOLTIP {a.Tooltip}");

            if (a.Effects.Count > 0)
            {
                sb.AppendLine(":EFFECTS");
                foreach (var e in a.Effects)
                    sb.AppendLine(e.ToLine());
            }

            File.WriteAllText(path, sb.ToString());
        }
    }
}
