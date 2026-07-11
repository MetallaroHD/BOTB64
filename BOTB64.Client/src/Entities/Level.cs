using BOTB64.Runtime;

namespace BOTB64.Entities
{ 
    public class Level : IReadable
    {
        public string Name { get; set; }
        public Board LevelBoard { get; set; }

        public static Level Load(string scriptPath, string modelPath, string wallPath, string envPath)
        {
            var reader = new LevelDataFile();
            DataFile df = new DataFile(scriptPath);
            Level level = reader.Read(df);

            level.LevelBoard.LoadModel(modelPath, wallPath, envPath);
            level.LevelBoard.Init();

            return level;
        }
    }
}
