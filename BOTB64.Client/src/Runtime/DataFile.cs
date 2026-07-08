namespace BOTB64.Runtime
{
    public class DataFile
    {
#if DEVELOPMENT
        //public static string DataDir = "C:\\Users\\caffetti.enrico\\Documents\\BOTB64\\BOTB64.Client\\data";
        public static string DataDir = "G:\\Dev\\BOTB64\\BOTB64.Client\\data";
        //public static string DataDir = "C:\\Users\\cafol\\Documents\\Development\\BOTB64\\BOTB64.Client\\data";
#else
        public static string DataDir = AppDomain.CurrentDomain.BaseDirectory + "data.b64";
#endif

        public string RelPath { get; set; }
        public string AbsPath { get; set; }

        public static bool DirectoryExists()
        {
            return System.IO.Path.Exists(DataDir);
        }

        public DataFile(string relPath) 
        {
            RelPath = relPath;
            AbsPath = DataDir + "\\" + relPath;
        }

        public bool Exists()
        {
#if DEVELOPMENT
            return File.Exists(AbsPath);
#else
            return ResourceArchive.Exists(AbsPath);
#endif
        }

        public string ReadAll()
        {
#if DEVELOPMENT
            return File.ReadAllText(AbsPath);
#else
            return ResourceArchive.ReadAllText(AbsPath);
#endif
        }

        public string[] ReadLines()
        {
#if DEVELOPMENT
            return File.ReadAllLines(AbsPath);
#else
            return ResourceArchive.ReadAllLines(AbsPath);
#endif
        }

        public bool WriteAll(string text)
        {
            if (!Exists())
                return false;

            File.WriteAllText(AbsPath, text);
            return true;
        }

        public bool WriteLines(string[] lines)
        {
            if (!Exists())
                return false;
            File.WriteAllLines(AbsPath, lines);
            return true;
        }

        public string GetDirectory()
        {
            return System.IO.Path.GetDirectoryName(AbsPath) + System.IO.Path.DirectorySeparatorChar;
        }

        // Casting to string
        public static explicit operator string(DataFile file) => file.AbsPath;
    }
}