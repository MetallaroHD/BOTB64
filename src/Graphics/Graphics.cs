using System.Numerics;
using RB = Raylib_cs.Raylib;
using RL = Raylib_cs;

namespace BOTB64.Graphics
{
    public static class Graphics
    {
        public static void Initialize(int width, int height, string title)
        {
            RB.InitWindow(width, height, title);
            RB.SetTargetFPS(60);
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

        public static void DrawModel(RL.Model model, Vector3 position, float scale, RL.Color tint)
        {
            RB.DrawModel(model, position, scale, tint);
        }
    }
}