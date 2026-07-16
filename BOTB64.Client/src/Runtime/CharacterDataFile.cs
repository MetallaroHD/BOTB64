using BOTB64.Entities;
using BOTB64.Shared.Files;
using System.Globalization;

namespace BOTB64.Runtime
{
    public class CharacterDataFile : BOTBDatafile<Character>
    {
        protected new const string HeaderTag = ":BOTB64CHARACTER";

        public override Character Read(DataFile df)
        {
            string[] lines = df.ReadLines();
            int i = SkipBlankAndComments(lines, 0);

            if (i >= lines.Length || !TryParseHeader(lines[i]))
                throw new FormatException($"'{(string)df}' is missing a valid header.");
            i++;

            var character = new Character();

            while (i < lines.Length)
            {
                i = SkipBlankAndComments(lines, i);
                if (i >= lines.Length) break;

                string line = lines[i].Trim();

                if (line.StartsWith(":AURAS"))
                {
                    i++;
                    i = ParseIntList(lines, i, character.PermanentAuras);
                }
                else if (line.StartsWith(":SPELLS"))
                {
                    i++;
                    i = ParseSpellLoadout(lines, i, character.SpellLoadout);
                }
                else if (line.StartsWith(":AUTOATTACK"))
                {
                    i++;
                    i = ParseAutoAttack(lines, i, character);
                }
                else if (line.StartsWith(":"))
                {
                    var (tag, rest) = SplitTag(line);
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

        private static void ApplyScalar(Character c, string tag, string rest)
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

        private static int ParseIntList(string[] lines, int i, List<int> target)
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

                target.Add(int.Parse(trimmed, CultureInfo.InvariantCulture));
                i++;
            }
            return i;
        }

        private static int ParseSpellLoadout(string[] lines, int i, Dictionary<int, int> target)
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
                int keybind = int.Parse(parts[0], CultureInfo.InvariantCulture);
                int spellId = int.Parse(parts[1], CultureInfo.InvariantCulture);
                target[keybind] = spellId;
                i++;
            }
            return i;
        }

        private static int ParseAutoAttack(string[] lines, int i, Character c)
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
                    throw new FormatException($"Malformed :AUTOATTACK line: '{trimmed}'");

                c.AutoAttackEffect = new Effect
                {
                    Script = parts[0],
                    Trigger = (EffectTrigger)int.Parse(parts[1], CultureInfo.InvariantCulture),
                    Source = (EffectSourceType)int.Parse(parts[2], CultureInfo.InvariantCulture),
                    Type = (EffectDamageType)int.Parse(parts[3], CultureInfo.InvariantCulture),
                    Scaling = (EffectDamageScaling)int.Parse(parts[4], CultureInfo.InvariantCulture)
                };

                i++;
                // only one autoattack effect expected; keep consuming/ignoring extras if present
            }
            return i;
        }

        private static (string Tag, string Value) SplitTag(string line)
        {
            int spaceIdx = line.IndexOf(' ');
            if (spaceIdx < 0)
                return (line.TrimStart(':'), "");
            return (line[..spaceIdx].TrimStart(':'), line[(spaceIdx + 1)..]);
        }

        public override bool TryRead(DataFile df, out Character data)
        {
            data = null;
            if (!df.Exists()) return false;
            try { data = Read(df); return true; }
            catch (FormatException) { return false; }
        }

        public override void Write(DataFile file, Character data) =>
            throw new NotImplementedException("Character editor not built yet.");
    }
}