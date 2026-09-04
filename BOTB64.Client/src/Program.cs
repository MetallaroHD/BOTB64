using BOTB64.Engine;
using BOTB64.Graphics;
using BOTB64.Graphics.UI;
using BOTB64.Graphics.Vfx;
using BOTB64.Runtime;
using BOTB64.Shared;
using BOTB64.Shared.Files;
using System.Diagnostics;

namespace BOTB64
{
    public class Program
    {
        // Marks the relaunched child process so it doesn't relaunch itself again.
        private const string RelaunchMarkerEnvVar = "BOTB64_RELAUNCHED";

        public static void Main()
        {
            if (BOTBVersion.Expires < DateTime.Now)
                return;

            if (!DataFile.DirectoryExists())
                return;

#if ENABLE_NATIVE_CRASH_DUMPS
            // Native/CLR-fatal crashes (access violations, etc.) bypass try/catch and
            // AppDomain.UnhandledException entirely. The only way to capture them is a
            // dotnet-level minidump, which the runtime only enables via environment
            // variables read at CoreCLR startup - too late to set from within Main().
            // So: relaunch ourselves once with those variables set, then just wait on
            // the child. Disable by removing ENABLE_NATIVE_CRASH_DUMPS from
            // BOTB64.Client.csproj if this ever causes trouble.
            if (Environment.GetEnvironmentVariable(RelaunchMarkerEnvVar) != "1" && TryRelaunchWithCrashDumps())
                return;
#endif

            RunGame();
        }

#if ENABLE_NATIVE_CRASH_DUMPS
        private static bool TryRelaunchWithCrashDumps()
        {
            try
            {
                var exePath = Environment.ProcessPath;
                if (string.IsNullOrEmpty(exePath))
                    return false;

                var dumpDir = Path.Combine(AppContext.BaseDirectory, "crashes", "dumps");
                Directory.CreateDirectory(dumpDir);

                var psi = new ProcessStartInfo
                {
                    FileName = exePath,
                    UseShellExecute = false,
                };
                psi.Environment[RelaunchMarkerEnvVar] = "1";
                psi.Environment["DOTNET_DbgEnableMiniDump"] = "1";
                // 2 = full private read/write memory (heaps + stacks) without read-only
                // mapped pages (loaded module images) - enough to debug a native fault
                // in WinDbg without ballooning to a multi-GB dump. Bump to 4 for a
                // complete memory dump if 2 isn't enough to diagnose something.
                psi.Environment["DOTNET_DbgMiniDumpType"] = "2";
                psi.Environment["DOTNET_DbgMiniDumpName"] = Path.Combine(dumpDir, "native_crash_%p.dmp");

                using var child = Process.Start(psi);
                if (child == null)
                    return false;

                child.WaitForExit();
                Environment.ExitCode = child.ExitCode;
                return true;
            }
            catch
            {
                // If relaunching fails for any reason, run directly rather than
                // refusing to start the game.
                return false;
            }
        }
#endif

        private static void RunGame()
        {
            Logger.InitPersistence();
            CrashReporter.Init();

            Settings.Load();
            UIRenderer.Update();
            ResourceManager.Initialize();
            ResourceArchive.Initialize(DataFile.DataDir);
            DatabaseFileManager.Init();
            VfxDatabase.Init();
            LuaEffectRunner.RegisterTypes();
            Graphics.Graphics.Initialize(1280, 720, "BOTB64");
            Engine.Engine.Initialize();

            while (!InputManager.ShouldClose())
            {
                try
                {
                    WindowManager.Update();
                    InputManager.NewFrame();
                    Engine.Engine.Update();
                    Engine.Engine.Render();
                }
                catch (Exception ex)
                {
                    var path = CrashReporter.Report(ex, "Main loop");
                    Console.Error.WriteLine($"BOTB64 crashed. Crash report saved to: {path}");
                    break;
                }
            }

            Graphics.Graphics.Unload();
            ResourceManager.ClearCache();
            CursorManager.Shutdown();
        }
    }
}
