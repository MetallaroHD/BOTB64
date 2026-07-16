using BOTB64.Shared.DTOs;
using System.Reflection.Emit;

namespace BOTB64.Shared.Files
{
    public static class DatabaseFileManager
    {
        public static readonly string AuraJSON = "Auras\\auras.json";
        public static readonly string SpellJSON = "Spells\\spells.json";
        public static readonly string CharacterJSON = "Characters\\characters.json";
        public static readonly string LevelJSON = "Levels\\levels.json";
        public static readonly string TileEffJSON = "TileEffects\\tileEffects.json";

        public static List<LevelDTO> Levels = new List<LevelDTO>();
        public static List<CharacterDTO> Characters = new List<CharacterDTO>();
        public static List<SpellDTO> Spells = new List<SpellDTO>();
        public static List<AuraDTO> Auras = new List<AuraDTO>();
        public static List<TileEffectDTO> TileEffects = new List<TileEffectDTO>();

        public static void Init()
        {
            JsonDataFile<LevelDTO> lf = new JsonDataFile<LevelDTO>();
            Levels = lf.DeserializeAll(new DataFile(LevelJSON));
            JsonDataFile<CharacterDTO> cf = new JsonDataFile<CharacterDTO>();
            Characters = cf.DeserializeAll(new DataFile(CharacterJSON));
            JsonDataFile<SpellDTO> sf = new JsonDataFile<SpellDTO>();
            Spells = sf.DeserializeAll(new DataFile(SpellJSON));
            JsonDataFile<AuraDTO> af = new JsonDataFile<AuraDTO>();
            Auras = af.DeserializeAll(new DataFile(AuraJSON));
            JsonDataFile<TileEffectDTO> tf = new JsonDataFile<TileEffectDTO>();
            TileEffects = tf.DeserializeAll(new DataFile(TileEffJSON));
        }
    }
}
