using System.IO;

namespace BOTB64.Editor.IO
{
    public enum FileKind
    {
        Unknown,
        Character,
        Spell,
        Aura,
        TileEffect
    }

    public static class FileKindDetector
    {
        public const string HeaderCharacter = ":BOTB64CHARACTER";
        public const string HeaderSpell = ":BOTB64SPELL";
        public const string HeaderAura = ":BOTB64AURA";
        public const string HeaderTileEffect = ":BOTB64TILEEFFECT";

        public static FileKind Detect(string path)
        {
            string[] lines = File.ReadAllLines(path);
            int i = BotbParsing.SkipBlankAndComments(lines, 0);
            if (i >= lines.Length)
                return FileKind.Unknown;

            string line = lines[i].Trim();

            if (line.StartsWith(HeaderCharacter, StringComparison.Ordinal)) return FileKind.Character;
            if (line.StartsWith(HeaderSpell, StringComparison.Ordinal)) return FileKind.Spell;
            if (line.StartsWith(HeaderAura, StringComparison.Ordinal)) return FileKind.Aura;
            if (line.StartsWith(HeaderTileEffect, StringComparison.Ordinal)) return FileKind.TileEffect;

            return FileKind.Unknown;
        }

        public static string ExtensionFor(FileKind kind) => kind switch
        {
            FileKind.Character => ".b64c",
            FileKind.Spell => ".b64s",
            FileKind.Aura => ".b64a",
            FileKind.TileEffect => ".b64t",
            _ => ""
        };
    }
}
