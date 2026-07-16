using BOTB64.Entities;
using BOTB64.Runtime;
using BOTB64.Shared.Files;
using System.Globalization;

public class AuraDataFile : ScriptDataFile<Aura>
{
    protected new const string HeaderTag = ":BOTB64AURA";

    public override Aura Read(DataFile df)
    {
        string[] lines = df.ReadLines();
        int i = SkipBlankAndComments(lines, 0);

        if (i >= lines.Length || !TryParseHeader(lines[i]))
            throw new FormatException($"'{(string)df}' is missing a valid header.");
        i++;

        var aura = new Aura();

        while (i < lines.Length)
        {
            i = SkipBlankAndComments(lines, i);
            if (i >= lines.Length) break;

            string line = lines[i].Trim();

            if (line.StartsWith(":PARAMETERS"))
            {
                i++;
                i = ParseParameters(lines, i, aura.Parameters);
            }
            else if (line.StartsWith(":EFFECTS"))
            {
                i++;
                i = ParseEffects(lines, i, aura);
            }
            else if (line.StartsWith(":"))
            {
                var (tag, rest) = SplitTag(line);
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

    private static void ApplyScalar(Aura aura, string tag, string rest)
    {
        switch (tag)
        {
            case "DURATION": aura.Duration = int.Parse(rest, CultureInfo.InvariantCulture); break;
            case "MAXSTACKS": aura.MaxStacks = int.Parse(rest, CultureInfo.InvariantCulture); break;
            case "DISPEL": aura.Dispel = (DispelType)int.Parse(rest, CultureInfo.InvariantCulture); break;
            case "TOOLTIP": aura.Tooltip = ParseQuotedOrRaw(rest); break;
        }
    }

    public override bool TryRead(DataFile df, out Aura data)
    {
        data = null;
        if (!df.Exists()) return false;
        try { data = Read(df); return true; }
        catch (FormatException) { return false; }
    }

    public override void Write(DataFile file, Aura data) =>
        throw new NotImplementedException("Aura editor not built yet.");
}