using BOTB64.Editor.Models;
using BOTB64.Engine;
using BOTB64.Entities;
using System.Globalization;
using System.IO;
using System.Text;

namespace BOTB64.Editor.IO
{
    public static class SpellIO
    {
        public static SpellModel Read(string path)
        {
            string[] lines = File.ReadAllLines(path);
            int i = BotbParsing.SkipBlankAndComments(lines, 0);

            if (i >= lines.Length || !lines[i].Trim().StartsWith(FileKindDetector.HeaderSpell, StringComparison.Ordinal))
                throw new FormatException($"'{path}' is missing a valid :BOTB64SPELL header.");
            i++;

            var spell = new SpellModel();

            while (i < lines.Length)
            {
                i = BotbParsing.SkipBlankAndComments(lines, i);
                if (i >= lines.Length) break;

                string line = lines[i].Trim();

                if (line.StartsWith(":PARAMETERS", StringComparison.Ordinal))
                {
                    i++;
                    i = BotbParsing.ParseParameters(lines, i, spell.Parameters);
                }
                else if (line.StartsWith(":EFFECTS", StringComparison.Ordinal))
                {
                    i++;
                    i = BotbParsing.ParseEffects(lines, i, spell.Effects);
                }
                else if (line.StartsWith(":", StringComparison.Ordinal))
                {
                    var (tag, rest) = BotbParsing.SplitTag(line);
                    ApplyScalar(spell, tag, rest);
                    i++;
                }
                else
                {
                    i++;
                }
            }

            return spell;
        }

        private static void ApplyScalar(SpellModel spell, string tag, string rest)
        {
            switch (tag)
            {
                case "RANGE": spell.Range = int.Parse(rest, CultureInfo.InvariantCulture); break;
                case "COOLDOWN": spell.Cooldown = int.Parse(rest, CultureInfo.InvariantCulture); break;
                case "CHARGES": spell.Charges = int.Parse(rest, CultureInfo.InvariantCulture); break;
                case "COST": spell.Cost = int.Parse(rest, CultureInfo.InvariantCulture); break;
                case "COSTPCT": spell.CostPct = float.Parse(rest, CultureInfo.InvariantCulture); break;
                case "CAST": spell.CastTime = int.Parse(rest, CultureInfo.InvariantCulture); break;
                case "PREP": spell.Preparation = int.Parse(rest, CultureInfo.InvariantCulture); break;
                case "TARGET": spell.ExplicitTarget = (TargetingType)int.Parse(rest, CultureInfo.InvariantCulture); break;
                case "SECRET": spell.Secret = rest.Trim().ToLowerInvariant() is "true" or "1" or "yes"; break;
                case "TRACKEDSOURCEAURA": spell.TrackedSourceAuraID = int.Parse(rest, CultureInfo.InvariantCulture); break;
                case "AREARADIUS": spell.AreaRadius = int.Parse(rest, CultureInfo.InvariantCulture); break;
                case "TOOLTIP": spell.Tooltip = BotbParsing.ParseQuotedOrRaw(rest); break;
            }
        }

        public static string Serialize(SpellModel s)
        {
            var sb = new StringBuilder();
            var inv = CultureInfo.InvariantCulture;

            sb.AppendLine(FileKindDetector.HeaderSpell);
            sb.AppendLine($":TARGET {((int)s.ExplicitTarget).ToString(inv)}");
            sb.AppendLine($":RANGE {s.Range.ToString(inv)}");
            sb.AppendLine($":COOLDOWN {s.Cooldown.ToString(inv)}");
            sb.AppendLine($":CHARGES {s.Charges.ToString(inv)}");
            sb.AppendLine($":COST {s.Cost.ToString(inv)}");
            sb.AppendLine($":COSTPCT {s.CostPct.ToString("G", inv)}");
            sb.AppendLine($":CAST {s.CastTime.ToString(inv)}");
            sb.AppendLine($":PREP {s.Preparation.ToString(inv)}");
            sb.AppendLine($":SECRET {(s.Secret ? "true" : "false")}");
            if (s.TrackedSourceAuraID != 0)
                sb.AppendLine($":TRACKEDSOURCEAURA {s.TrackedSourceAuraID.ToString(inv)}");
            if (s.AreaRadius != 0)
                sb.AppendLine($":AREARADIUS {s.AreaRadius.ToString(inv)}");

            if (s.Parameters.Count > 0)
            {
                sb.AppendLine(":PARAMETERS");
                foreach (var p in s.Parameters)
                    sb.AppendLine($"{p.Name} {((int)p.Type).ToString(inv)} {p.ToRaw()}");
            }

            sb.AppendLine($":TOOLTIP {s.Tooltip}");

            if (s.Effects.Count > 0)
            {
                sb.AppendLine(":EFFECTS");
                foreach (var e in s.Effects)
                    sb.AppendLine(e.ToLine());
            }

            return sb.ToString();
        }

        public static void Write(string path, SpellModel s) => File.WriteAllText(path, Serialize(s));
    }
}
