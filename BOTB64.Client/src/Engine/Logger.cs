using BOTB64.Graphics.UI;

namespace BOTB64.Engine
{
    public static class Logger
    {
        public static LogArea? WriteArea;

        // Kept independent of WriteArea so events logged outside of gameplay
        // (lobby, netcode, etc.) are still captured for crash reports.
        private const int RingCapacity = 500;
        private static readonly Queue<string> RecentLines = new();
        private static readonly object LogLock = new();

        private static StreamWriter? FileWriter;

        public static string? LogFilePath { get; private set; }

        public static void InitPersistence()
        {
            try
            {
                var logDir = Path.Combine(AppContext.BaseDirectory, "logs");
                Directory.CreateDirectory(logDir);

                LogFilePath = Path.Combine(logDir, $"log_{DateTime.Now:yyyyMMdd_HHmmss}.txt");
                FileWriter = new StreamWriter(LogFilePath, append: false) { AutoFlush = true };
            }
            catch
            {
                // Disk unavailable/read-only - keep running with in-memory logging only.
                FileWriter = null;
            }
        }

        public static void Init(LogArea area)
        {
            WriteArea = area;
        }

        public static void Log(string message)
        {
            var line = $"[{DateTime.Now:HH:mm:ss.fff}] {message}";

            lock (LogLock)
            {
                RecentLines.Enqueue(line);
                while (RecentLines.Count > RingCapacity)
                    RecentLines.Dequeue();

                try
                {
                    FileWriter?.WriteLine(line);
                }
                catch
                {
                    // best-effort disk logging only
                }
            }

            WriteArea?.Append(message);
        }

        public static IReadOnlyList<string> GetRecentLines()
        {
            lock (LogLock)
                return RecentLines.ToArray();
        }

        public static void Update()
        {
            WriteArea?.Update();
        }

        public static void Unload()
        {
            WriteArea = null;
        }
    }
}
