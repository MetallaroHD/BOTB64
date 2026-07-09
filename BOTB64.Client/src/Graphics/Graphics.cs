using System.Numerics;
using RB = Raylib_cs.Raylib;
using RL = Raylib_cs;

namespace BOTB64.Graphics
{
    public static class Graphics
    {
        public static void Initialize(int width, int height, string title)
        {
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
            RB.EndDrawing();
        }

        public static void Unload()
        {
            RB.CloseWindow();
        }
    }
}