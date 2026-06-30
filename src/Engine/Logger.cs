using BOTB64.Graphics.UI;

namespace BOTB64.Engine
{
    public static class Logger
    {
        public static LogArea? WriteArea;

        public static void Init(LogArea area)
        {
            WriteArea = area;
        }

        public static void Log(string message)
        {
            if (WriteArea == null)
            {
                Console.WriteLine("Log area not bound!");
                return;
            }
            WriteArea.Append(message);
        }

        public static void Update()
        {
            if (WriteArea == null)
            {
                return;
            }
            WriteArea.Update();
        }

        public static void Unload() 
        {
            WriteArea = null;
        }
    }
}
