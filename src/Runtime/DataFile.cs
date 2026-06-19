#define DEVELOPMENT

namespace BOTB64.Runtime
{
    public class DataFile
    {
#if DEVELOPMENT
        public static string DataDir = "C:\\Users\\caffetti.enrico\\Documents\\BOTB64\\data\\";
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

        // Casting to string
        public static explicit operator string(DataFile file) => file.Path;
    }
}