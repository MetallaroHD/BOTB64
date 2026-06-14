#define DEVELOPMENT

namespace BOTB64.Core
{
    public class DataFile
    {
#if DEVELOPMENT
        public static string DataDir = "G:\\Dev\\BOTB64\\data\\";
#else
        public static string DataDir = System.Environment.ProcessPath;
#endif

        public string Path { get; set; }

        public DataFile(string relPath) 
        {
            Path = DataDir + relPath;
        }

        // Casting to string
        public static explicit operator string(DataFile file) => file.Path;
    }
}