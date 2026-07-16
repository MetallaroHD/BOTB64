using System.Globalization;
using System.Text;
using BOTB64.Editor.Models;
using BOTB64.Entities;
using System.IO;

namespace BOTB64.Editor.IO
{
    public static class CharacterIO
    {
        public static CharacterModel Read(string path)
        {
            string[] lines = File.ReadAllLines(path);
            int i = BotbParsing.SkipBlankAndComments(lines, 0);

            if (i >= lines.Length || !lines[i].Trim().StartsWith(FileKindDetector.HeaderCharacter, StringComparison.Ordinal))
                throw new FormatException($"'{path}' is missing a valid :BOTB64CHARACTER header.");
            i++;

            var character = new CharacterModel();

            while (i < lines.Length)
            {
                i = BotbParsing.SkipBlankAndComments(lines, i);
                if (i >= lines.Length) break;

                string line = lines[i].Trim();

                if (line.StartsWith(":AURAS", StringComparison.Ordinal))
                {
                    i++;
                    i = BotbParsing.ParseIntList(lines, i, character.PermanentAuras);
                }
                else if (line.StartsWith(":SPELLS", StringComparison.Ordinal))
                {
                    i++;
                    i = BotbParsing.ParseSpellLoadout(lines, i, character.SpellLoadout);
                }
                else if (line.StartsWith(":AUTOATTACK", StringComparison.Ordinal)
                         || line.StartsWith(":EFFECTS", StringComparison.Ordinal))
                {
                    i++;
                    i = BotbParsing.ParseAutoAttack(lines, i, character);
                }
                else if (line.StartsWith(":", StringComparison.Ordinal))
                {
                    var (tag, rest) = BotbParsing.SplitTag(line);
                    ApplyScalar(character, tag, rest);
                    i++;
                }
                else
                {
                    i++;
                }
            }

            return character;
        }

        private static void ApplyScalar(CharacterModel c, string tag, string rest)
        {
            switch (tag)
            {
                case "MAXHP": c.MaxHP = int.Parse(rest, CultureInfo.InvariantCulture); break;
                case "MAXRES": c.MaxRes = int.Parse(rest, CultureInfo.InvariantCulture); break;
                case "HPREGEN": c.HPRegen = int.Parse(rest, CultureInfo.InvariantCulture); break;
                case "RESREGEN": c.ResRegen = int.Parse(rest, CultureInfo.InvariantCulture); break;
                case "STARTRES": c.StartRes = int.Parse(rest, CultureInfo.InvariantCulture); break;
                case "ATTACKPOWER": c.AttackPower = int.Parse(rest, CultureInfo.InvariantCulture); break;
                case "SPELLPOWER": c.SpellPower = int.Parse(rest, CultureInfo.InvariantCulture); break;
                case "DEFENSE": c.Defense = int.Parse(rest, CultureInfo.InvariantCulture); break;
                case "MAGDEFENSE": c.MagicDefense = int.Parse(rest, CultureInfo.InvariantCulture); break;
                case "HASTE": c.Haste = int.Parse(rest, CultureInfo.InvariantCulture); break;
                case "SPEED": c.Speed = int.Parse(rest, CultureInfo.InvariantCulture); break;
                case "ARMORPEN": c.ArmorPen = float.Parse(rest, CultureInfo.InvariantCulture); break;
                case "SPELLPEN": c.SpellPen = float.Parse(rest, CultureInfo.InvariantCulture); break;
                case "CRIT": c.Crit = float.Parse(rest, CultureInfo.InvariantCulture); break;
                case "LIFESTEAL": c.LifeSteal = float.Parse(rest, CultureInfo.InvariantCulture); break;
                case "SPELLVAMP": c.SpellVamp = float.Parse(rest, CultureInfo.InvariantCulture); break;
                case "RESTYPE": c.ResType = (ResourceType)int.Parse(rest, CultureInfo.InvariantCulture); break;
                case "AARANGE": c.AutoAttackRange = int.Parse(rest, CultureInfo.InvariantCulture); break;
                case "AAAP": c.AutoAttackAP = float.Parse(rest, CultureInfo.InvariantCulture); break;
                case "AASP": c.AutoAttackSP = float.Parse(rest, CultureInfo.InvariantCulture); break;
            }
        }

        public static void Write(string path, CharacterModel c)
        {
            var sb = new StringBuilder();
            var inv = CultureInfo.InvariantCulture;

            sb.AppendLine(FileKindDetector.HeaderCharacter);
            sb.AppendLine($":MAXHP {c.MaxHP.ToString(inv)}");
            sb.AppendLine($":MAXRES {c.MaxRes.ToString(inv)}");
            sb.AppendLine($":RESTYPE {((int)c.ResType).ToString(inv)}");
            sb.AppendLine($":HPREGEN {c.HPRegen.ToString(inv)}");
            sb.AppendLine($":RESREGEN {c.ResRegen.ToString(inv)}");
            sb.AppendLine($":STARTRES {c.StartRes.ToString(inv)}");
            sb.AppendLine($":ATTACKPOWER {c.AttackPower.ToString(inv)}");
            sb.AppendLine($":SPELLPOWER {c.SpellPower.ToString(inv)}");
            sb.AppendLine($":DEFENSE {c.Defense.ToString(inv)}");
            sb.AppendLine($":MAGDEFENSE {c.MagicDefense.ToString(inv)}");
            sb.AppendLine($":HASTE {c.Haste.ToString(inv)}");
            sb.AppendLine($":SPEED {c.Speed.ToString(inv)}");
            sb.AppendLine($":ARMORPEN {c.ArmorPen.ToString("G", inv)}");
            sb.AppendLine($":SPELLPEN {c.SpellPen.ToString("G", inv)}");
            sb.AppendLine($":CRIT {c.Crit.ToString("G", inv)}");
            sb.AppendLine($":LIFESTEAL {c.LifeSteal.ToString("G", inv)}");
            sb.AppendLine($":SPELLVAMP {c.SpellVamp.ToString("G", inv)}");
            sb.AppendLine($":AARANGE {c.AutoAttackRange.ToString(inv)}");
            sb.AppendLine($":AAAP {c.AutoAttackAP.ToString("G", inv)}");
            sb.AppendLine($":AASP {c.AutoAttackSP.ToString("G", inv)}");

            sb.AppendLine(":EFFECTS");
            sb.AppendLine(c.AutoAttackEffect.ToLine());

            sb.AppendLine(":AURAS");
            foreach (var a in c.PermanentAuras)
                sb.AppendLine(a.Value.ToString(inv));

            sb.AppendLine(":SPELLS");
            foreach (var s in c.SpellLoadout)
                sb.AppendLine($"{s.Keybind.ToString(inv)} {s.SpellId.ToString(inv)}");

            File.WriteAllText(path, sb.ToString());
        }
    }
}
