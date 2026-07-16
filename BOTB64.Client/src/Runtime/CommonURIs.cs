using BOTB64.Entities;
using BOTB64.Shared.DTOs;

namespace BOTB64.Runtime
{
    public static class CommonURIs
    {
        public static readonly string AuraDir = "Auras\\";
        public static readonly string SpellDir = "Spells\\";
        public static readonly string CharacterDir = "Characters\\";
        public static readonly string LevelDir = "Levels\\";
        public static readonly string TileEffDir = "TileEffects\\";
        public static readonly string GraphicsDir = "Graphics\\";
        public static readonly string ScriptDir = "Scripts\\";
        public static readonly string GraphicsIconDir = "Graphics\\Icons\\";
        public static readonly string GraphicsAnimDir = "Graphics\\Animations\\";
        public static readonly string GraphicsModelDir = "Graphics\\Models\\";

        public static readonly string AuraEXT = ".b64a";
        public static readonly string SpellEXT = ".b64s";
        public static readonly string CharacterEXT = ".b64c";
        public static readonly string LevelEXT = ".b64m";
        public static readonly string TileEffEXT = ".b64t";

        public static readonly string ModelEXT = ".gltf";
        public static readonly string ShaderVSEXT = ".vs";
        public static readonly string ShaderFSEXT = ".fs";
        public static readonly string ImageEXT = ".png";
        public static readonly string ScriptExt = ".lua";

        public static (string script, string model, string wall, string env, string shaderv, string shaderf) GetLevelResources(LevelDTO level) 
        {
            string scriptURI = LevelDir + level.Subdir + "\\" + level.Script + LevelEXT;
            string modelURI = LevelDir + level.Subdir + "\\" + level.Model + ModelEXT;
            string wallURI = LevelDir + level.Subdir + "\\" + level.Wall + ModelEXT;
            string environmentURI = LevelDir + level.Subdir + "\\" + level.Environment + ModelEXT;
            string shaderv = LevelDir + level.Subdir + "\\" + level.Shader + ShaderVSEXT;
            string shaderf = LevelDir + level.Subdir + "\\" + level.Shader + ShaderFSEXT;
            return (scriptURI, modelURI, wallURI, environmentURI, shaderv, shaderf);
        }
        public static (string script, string model, string icon) GetCharacterResources(CharacterDTO character)
        {
            string scriptURI = CharacterDir + character.Subdir + "\\" + character.ScriptURI + CharacterEXT;

            return (scriptURI, GetCharacterModel(character), GetCharacterIcon(character));
        }
        public static string GetCharacterIcon(CharacterDTO character)
        {
            return CharacterDir + character.Subdir + "\\" + character.IconURI + ImageEXT;
        }

        public static string GetCharacterModel(CharacterDTO character)
        {
            return CharacterDir + character.Subdir + "\\" + character.ModelURI + ModelEXT;
        }

        public static string GetAuraIcon(AuraDTO aura)
        {
            return GraphicsIconDir + aura.IconURI + ImageEXT;
        }
    }
}