using BOTB64.Graphics.UI;
using RB = Raylib_cs.Raylib;
using RL = Raylib_cs;

namespace BOTB64.Graphics
{
    public static class WindowManager
    {
        private static bool lastFullscreen;

        public static void Update()
        {
            if (Settings.FullScreen != lastFullscreen)
            {
                ToggleFullscreen();
                lastFullscreen = Settings.FullScreen;
            }

            UIRenderer.Update();
        }


        private static void ToggleFullscreen()
        {
            RB.ToggleFullscreen();
        }

        public static void SetBorderlessFullscreen(bool enabled)
        {
            if (enabled)
            {
                RB.SetWindowState(RL.ConfigFlags.BorderlessWindowMode);

                int monitor = RB.GetCurrentMonitor();

                RB.SetWindowSize(
                    RB.GetMonitorWidth(monitor),
                    RB.GetMonitorHeight(monitor)
                );

                RB.SetWindowPosition(0, 0);
            }
            else
            {
                RB.ClearWindowState(RL.ConfigFlags.BorderlessWindowMode);

                RB.SetWindowSize(1280, 720);
                RB.SetWindowPosition(100, 100);
            }
        }
    }
}
