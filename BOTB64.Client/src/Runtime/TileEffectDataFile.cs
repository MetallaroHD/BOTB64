using BOTB64.Entities;
using BOTB64.Runtime;
using BOTB64.Shared.Files;
using System.Globalization;

public class TileEffectDataFile : ScriptDataFile<TileEffect>
{
    protected new const string HeaderTag = ":BOTB64TILEEFFECT";

    public override TileEffect Read(DataFile df)
    {
        string[] lines = df.ReadLines();
        int i = SkipBlankAndComments(lines, 0);

        if (i >= lines.Length || !TryParseHeader(lines[i]))
            throw new FormatException($"'{(string)df}' is missing a valid header.");
        i++;

        var fx = new TileEffect();

        while (i < lines.Length)
        {
            i = SkipBlankAndComments(lines, i);
            if (i >= lines.Length) break;

            string line = lines[i].Trim();

            if (line.StartsWith(":PARAMETERS"))
            {
                i++;
                i = ParseParameters(lines, i, fx.Parameters);
            }
            else if (line.StartsWith(":EFFECTS"))
            {
                i++;
                i = ParseEffects(lines, i, fx);
            }
            else if (line.StartsWith(":"))
            {
                var (tag, rest) = SplitTag(line);
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

    private static void ApplyScalar(TileEffect fx, string tag, string rest)
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

    public override bool TryRead(DataFile df, out TileEffect data)
    {
        data = null;
        if (!df.Exists()) return false;
        try { data = Read(df); return true; }
        catch (FormatException) { return false; }
    }

    public override void Write(DataFile file, TileEffect data) =>
        throw new NotImplementedException("TileEffect editor not built yet.");
}