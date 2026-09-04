using BOTB64.Shared;
using System.Text;

namespace BOTB64.Engine
{
    public static class CrashReporter
    {
        private static string CrashDir => Path.Combine(AppContext.BaseDirectory, "crashes");

        // Hooks the backstops for exceptions the main-loop try/catch can't see
        // (background threads, e.g. the netcode poll loop, finalizer thread, etc.).
        public static void Init()
        {
            AppDomain.CurrentDomain.UnhandledException += (_, e) =>
            {
                Report(e.ExceptionObject as Exception, "AppDomain.UnhandledException");
            };

            TaskScheduler.UnobservedTaskException += (_, e) =>
            {
                Logger.Log($"Unobserved task exception: {e.Exception.GetBaseException()}");
                e.SetObserved();
            };
        }

        public static string Report(Exception? ex, string source)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"BOTB64 crash report - {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine($"Version: {BOTBVersion.Major}.{BOTBVersion.Minor}.{BOTBVersion.Patch}{BOTBVersion.Special}");
            sb.AppendLine($"Source: {source}");
            sb.AppendLine($"State: {StateManager.CurrentStateName}");
            sb.AppendLine($"OS: {Environment.OSVersion}, 64-bit process: {Environment.Is64BitProcess}");
            sb.AppendLine();
            sb.AppendLine("Exception:");
            sb.AppendLine(ex?.ToString() ?? "(no exception object)");
            sb.AppendLine();

            var recent = Logger.GetRecentLines();
            sb.AppendLine($"--- Last {recent.Count} log lines ---");
            foreach (var line in recent)
                sb.AppendLine(line);

            var report = sb.ToString();
            var path = "(not written)";

            try
            {
                Directory.CreateDirectory(CrashDir);
                path = Path.Combine(CrashDir, $"crash_{DateTime.Now:yyyyMMdd_HHmmss}.txt");
                File.WriteAllText(path, report);
            }
            catch
            {
                // best effort only - still surfaced via stderr below
            }

            Console.Error.WriteLine(report);
            return path;
        }
    }
}
