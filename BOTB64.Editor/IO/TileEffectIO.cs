using System.Globalization;
using System.Text;
using BOTB64.Editor.Models;
using BOTB64.Entities;
using System.IO;

namespace BOTB64.Editor.IO
{
    public static class TileEffectIO
    {
        public static TileEffectModel Read(string path)
        {
            string[] lines = File.ReadAllLines(path);
            int i = BotbParsing.SkipBlankAndComments(lines, 0);

            if (i >= lines.Length || !lines[i].Trim().StartsWith(FileKindDetector.HeaderTileEffect, StringComparison.Ordinal))
                throw new FormatException($"'{path}' is missing a valid :BOTB64TILEEFFECT header.");
            i++;

            var fx = new TileEffectModel();

            while (i < lines.Length)
            {
                i = BotbParsing.SkipBlankAndComments(lines, i);
                if (i >= lines.Length) break;

                string line = lines[i].Trim();

                if (line.StartsWith(":PARAMETERS", StringComparison.Ordinal))
                {
                    i++;
                    i = BotbParsing.ParseParameters(lines, i, fx.Parameters);
                }
                else if (line.StartsWith(":EFFECTS", StringComparison.Ordinal))
                {
                    i++;
                    i = BotbParsing.ParseEffects(lines, i, fx.Effects);
                }
                else if (line.StartsWith(":", StringComparison.Ordinal))
                {
                    var (tag, rest) = BotbParsing.SplitTag(line);
                    ApplyScalar(fx, tag, rest);
                    i++;
                }
                else
                {
                    i++;
                }
            }

            return fx;
        }

        private static void ApplyScalar(TileEffectModel fx, string tag, string rest)
        {
            switch (tag)
            {
                case "DURATION": fx.Duration = int.Parse(rest, CultureInfo.InvariantCulture); break;
                case "DISPEL": fx.Dispel = (DispelType)int.Parse(rest, CultureInfo.InvariantCulture); break;
                case "TILETYPE": fx.TileType = (TileEffectApplicableTile)int.Parse(rest, CultureInfo.InvariantCulture); break;
                case "FLAGS": fx.Flags = (TileEffectFlag)int.Parse(rest, CultureInfo.InvariantCulture); break;
                case "TYPE": fx.Type = (TileEffectType)int.Parse(rest, CultureInfo.InvariantCulture); break;
            }
        }

        public static void Write(string path, TileEffectModel fx)
        {
            var sb = new StringBuilder();
            var inv = CultureInfo.InvariantCulture;

            sb.AppendLine(FileKindDetector.HeaderTileEffect);
            sb.AppendLine($":DURATION {fx.Duration.ToString(inv)}");
            sb.AppendLine($":DISPEL {((int)fx.Dispel).ToString(inv)}");
            sb.AppendLine($":TILETYPE {((int)fx.TileType).ToString(inv)}");
            sb.AppendLine($":FLAGS {((int)fx.Flags).ToString(inv)}");
            sb.AppendLine($":TYPE {((int)fx.Type).ToString(inv)}");

            if (fx.Parameters.Count > 0)
            {
                sb.AppendLine(":PARAMETERS");
                foreach (var p in fx.Parameters)
                    sb.AppendLine($"{p.Name} {((int)p.Type).ToString(inv)} {p.ToRaw()}");
            }

            if (fx.Effects.Count > 0)
            {
                sb.AppendLine(":EFFECTS");
                foreach (var e in fx.Effects)
                    sb.AppendLine(e.ToLine());
            }

            File.WriteAllText(path, sb.ToString());
        }
    }
}
