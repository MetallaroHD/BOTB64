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
                SetBorderlessFullscreen(Settings.FullScreen);
                lastFullscreen = Settings.FullScreen;
            }

            if (Settings.FullScreen)
            {
                if (!RB.IsWindowFocused() && !RB.IsWindowMinimized())
                {
                    RB.MinimizeWindow();
                }
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

                Settings.Scale = Math.Min((float)RB.GetMonitorWidth(monitor)/1280, (float)RB.GetMonitorHeight(monitor) / 720);

                RB.SetWindowSize(RB.GetMonitorWidth(monitor), RB.GetMonitorHeight(monitor));

                RB.SetWindowPosition(0, 0);
            }
            else
            {
                RB.ClearWindowState(RL.ConfigFlags.BorderlessWindowMode);
            }
        }
    }
}
