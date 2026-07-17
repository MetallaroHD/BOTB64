using BOTB64.Engine;
using BOTB64.Entities;
using BOTB64.Shared.Files;
using System.Globalization;

namespace BOTB64.Runtime
{
    public class SpellDataFile : ScriptDataFile<Spell>
    {
        protected new const string HeaderTag = ":BOTB64SPELL";

        public override Spell Read(DataFile df)
        {
            string[] lines = df.ReadLines();
            int i = SkipBlankAndComments(lines, 0);

            if (i >= lines.Length || !TryParseHeader(lines[i]))
                throw new FormatException($"'{(string)df}' is missing a valid header.");
            i++;

            var spell = new Spell();

            while (i < lines.Length)
            {
                i = SkipBlankAndComments(lines, i);
                if (i >= lines.Length) break;

                string line = lines[i].Trim();

                if (line.StartsWith(":PARAMETERS"))
                {
                    i++;
                    i = ParseParameters(lines, i, spell.Parameters);
                }
                else if (line.StartsWith(":EFFECTS"))
                {
                    i++;
                    i = ParseEffects(lines, i, spell);
                }
                else if (line.StartsWith(":"))
                {
                    var (tag, rest) = SplitTag(line);
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

        private static void ApplyScalar(Spell spell, string tag, string rest)
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
                case "TOOLTIP": spell.Tooltip = ParseQuotedOrRaw(rest); break;
            }
        }

        public override bool TryRead(DataFile df, out Spell data)
        {
            data = null;
            if (!df.Exists()) return false;
            try { data = Read(df); return true; }
            catch (FormatException) { return false; }
        }

        public override void Write(DataFile file, Spell data)
        {
            throw new NotImplementedException("Spell editor not built yet.");
        }
    }
}