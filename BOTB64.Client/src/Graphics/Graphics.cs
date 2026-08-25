using BOTB64.Runtime;
using RB = Raylib_cs.Raylib;
using RL = Raylib_cs;

namespace BOTB64.Graphics
{
    public static class Graphics
    {
        public static void Initialize(int width, int height, string title)
        {
            if (Settings.VSync)
                RB.SetConfigFlags(RL.ConfigFlags.VSyncHint);
            RB.InitWindow((int)(width * Settings.Scale), (int)(height * Settings.Scale), title);
            RB.SetTargetFPS(60);
            RB.SetExitKey(RL.KeyboardKey.Null);
        }

        public static void BeginFrame()
        {
            RB.BeginDrawing();
            RB.ClearBackground(RL.Color.SkyBlue);
        }

        public static void EndFrame()
        {
#if DEBUG
            //Console.WriteLine(RB.GetFrameTime());
#endif
            RB.EndDrawing();
        }

        public static void Unload()
        {
            RB.CloseWindow();
        }

        public static void ApplyScale(float scale)
        {
            Settings.Scale = scale;
            int width = (int)(1280 * scale);
            int height = (int)(720 * scale);
            RB.SetWindowSize(width, height);
        }

        public static void ApplyVSync(bool enabled)
        {
            Settings.VSync = enabled;
            if (enabled)
                RB.SetWindowState(RL.ConfigFlags.VSyncHint);
            else
                RB.ClearWindowState(RL.ConfigFlags.VSyncHint);
        }
    }
}