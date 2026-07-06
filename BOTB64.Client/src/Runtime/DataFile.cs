#define DEVELOPMENT

namespace BOTB64.Runtime
{
    public class DataFile
    {
#if DEVELOPMENT
        public static string DataDir = "C:\\Users\\caffetti.enrico\\Documents\\BOTB64\\BOTB64.Client\\data\\";
        //public static string DataDir = "G:\\Dev\\BOTB64\\BOTB64.Client\\data\\";
#else
        public static string DataDir = System.Environment.ProcessPath;
#endif

        public string Path { get; set; }

        public DataFile(string relPath) 
        {
            Path = DataDir + relPath;
        }

        public bool Exists()
        {
            return File.Exists(Path);
        }

        public bool WriteAll(string text)
        {
            if (!Exists())
                return false;

            File.WriteAllText(Path, text);
            return true;
        }

        public string ReadAll()
        {
            if (!Exists())
                return "";

            return File.ReadAllText(Path);
        }

        public bool WriteLines(string[] lines)
        {
            if (!Exists())
                return false;
            File.WriteAllLines(Path, lines);
            return true;
        }

        public string[] ReadLines()
        { 
            if (!Exists())
                return [];
            return File.ReadAllLines(Path);
        }

        public string GetDirectory()
        {
            return System.IO.Path.GetDirectoryName(Path) + System.IO.Path.DirectorySeparatorChar;
        }

        // Casting to string
        public static explicit operator string(DataFile file) => file.Path;
    }
}