using IniParser;
using IniParser.Model;
using System.Globalization;

namespace BOTB64
{
    public static class Settings
    {
        /* GRAPHICS */
        // this is the size relative to 1280x720 - for example 1920x1080 is 1.5f
        public static float Scale = 1.0f;
        public static bool FullScreen = false;
        public static bool VSync = true;

        /* GAMEPLAY */
        public static bool AskEndTurn = true;

        private static readonly string FileName = "config.ini";

        public static void Load()
        {
            if (!File.Exists(FileName))
            {
                Save();
                return;
            }

            try 
            {
                var parser = new FileIniDataParser();
                IniData data = parser.ReadFile(FileName);

                Scale = float.Parse(data["Graphics"]["Scale"], CultureInfo.InvariantCulture);
                FullScreen = bool.Parse(data["Graphics"]["Fullscreen"]);
                VSync = bool.Parse(data["Graphics"]["VSync"]);
                AskEndTurn = bool.Parse(data["Gameplay"]["AskEndTurn"]);
            }
            catch 
            {
                //invalid data, we purge and rewrite
                File.Delete(FileName);
                Save();
            }

        }

        public static void Save()
        {
            var data = new IniData();

            data["Graphics"]["Scale"] = Scale.ToString(CultureInfo.InvariantCulture);
            data["Graphics"]["Fullscreen"] = FullScreen.ToString();
            data["Graphics"]["VSync"] = VSync.ToString();
            data["Gameplay"]["AskEndTurn"] = AskEndTurn.ToString();

            var parser = new FileIniDataParser();
            parser.WriteFile(FileName, data);
        }
    }
}
