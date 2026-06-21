using BOTB64.Runtime;

namespace BOTB64.Entities
{
    public class Level
    {
        public string Name { get; set; }
        public Board LevelBoard { get; set; }

        public static Level Load(string path)
        {
            var reader = new LevelDataFile();
            DataFile df = new DataFile(path);
            Level level = reader.Read(df);

            string dir = Path.GetDirectoryName(path) ?? string.Empty;
            string modelPath = Path.Combine(dir, "board.gltf");
            level.LevelBoard.LoadModel(modelPath);
            level.LevelBoard.Init();

            return level;
        }
    }
}
