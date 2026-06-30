namespace BOTB64.Runtime
{
    public static class CommonURIs
    {
        public static readonly string AuraJSON = "Auras\\auras.json";
        public static readonly string SpellJSON = "Spells\\spells.json";
        public static readonly string CharacterJSON = "Characters\\characters.json";
        public static readonly string LevelJSON = "Levels\\levels.json";
        public static readonly string TileEffJSON = "TileEffects\\tileEffects.json";

        public static readonly DataFile AuraJSONF = new DataFile(AuraJSON);
        public static readonly DataFile SpellJSONF = new DataFile(SpellJSON);
        public static readonly DataFile CharacterJSONF = new DataFile(CharacterJSON);
        public static readonly DataFile LevelJSONF = new DataFile(LevelJSON);
        public static readonly DataFile TileEffJSONF = new DataFile(TileEffJSON);

        public static readonly string AuraEXT = ".b64a";
        public static readonly string SpellEXT = ".b64s";
        public static readonly string CharacterEXT = ".b64c";
        public static readonly string LevelEXT = ".b64m";
        public static readonly string TileEffEXT = ".b64t";

        public static readonly string ModelEXT = ".gltf";
    }
}