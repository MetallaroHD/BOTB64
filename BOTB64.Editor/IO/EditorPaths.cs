using System.IO;
using BOTB64.Shared.DTOs;

namespace BOTB64.Editor.IO
{
    // Mirrors the folder layout in BOTB64.Runtime.CommonURIs, but resolved
    // against a user-chosen "data root" folder instead of the compile-time
    // DataFile.DataDir (which is gated behind the DEVELOPMENT symbol and
    // ResourceArchive - both meant for the shipped client, not the editor).
    public static class EditorPaths
    {
        public static string CharactersDir(string root) => Path.Combine(root, "Characters");
        public static string SpellsDir(string root) => Path.Combine(root, "Spells");
        public static string AurasDir(string root) => Path.Combine(root, "Auras");
        public static string TileEffectsDir(string root) => Path.Combine(root, "TileEffects");

        public static string GraphicsIconsDir(string root) => Path.Combine(root, "Graphics", "Icons");
        public static string GraphicsModelsDir(string root) => Path.Combine(root, "Graphics", "Models");
        public static string GraphicsAnimationsDir(string root) => Path.Combine(root, "Graphics", "Animations");

        public static string CharactersJson(string root) => Path.Combine(CharactersDir(root), "characters.json");
        public static string SpellsJson(string root) => Path.Combine(SpellsDir(root), "spells.json");
        public static string AurasJson(string root) => Path.Combine(AurasDir(root), "auras.json");
        public static string TileEffectsJson(string root) => Path.Combine(TileEffectsDir(root), "tileEffects.json");

        public static string CharacterSubdirPath(string root, string subdir) =>
            Path.Combine(CharactersDir(root), subdir ?? "");

        public static string CharacterScriptPath(string root, CharacterDTO c) =>
            Path.Combine(CharacterSubdirPath(root, c.Subdir), (string.IsNullOrEmpty(c.ScriptURI) ? "character" : c.ScriptURI) + ".b64c");

        public static string CharacterModelPath(string root, CharacterDTO c) =>
            Path.Combine(CharacterSubdirPath(root, c.Subdir), (c.ModelURI ?? "") + ".glb");

        public static string CharacterIconPath(string root, CharacterDTO c) =>
            Path.Combine(CharacterSubdirPath(root, c.Subdir), (c.IconURI ?? "") + ".png");

        public static string SpellScriptPath(string root, SpellDTO s) =>
            Path.Combine(SpellsDir(root), (string.IsNullOrEmpty(s.ScriptURI) ? "spell" : s.ScriptURI) + ".b64s");

        public static string AuraScriptPath(string root, AuraDTO a) =>
            Path.Combine(AurasDir(root), (string.IsNullOrEmpty(a.ScriptURI) ? "aura" : a.ScriptURI) + ".b64a");

        public static string TileEffectScriptPath(string root, TileEffectDTO t) =>
            Path.Combine(TileEffectsDir(root), (string.IsNullOrEmpty(t.ScriptURI) ? "tileeffect" : t.ScriptURI) + ".b64t");
    }
}
