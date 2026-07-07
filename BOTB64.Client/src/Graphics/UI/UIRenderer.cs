using System.Numerics;
using RB = Raylib_cs.Raylib;
using RL = Raylib_cs;

namespace BOTB64.Graphics.UI
{
    public static class UIRenderer
    {
        public static RL.Camera2D Camera;

        public static void Update()
        {
            float scale = Settings.Scale;

            Camera = new RL.Camera2D
            {
                Target = Vector2.Zero,
                Offset = new Vector2((RB.GetScreenWidth() - (1280 * scale)) / 2f, (RB.GetScreenHeight() - (720 * scale)) / 2f),
                Rotation = 0f,
                Zoom = scale
            };
        }

        public static void Begin()
        {
            RB.BeginMode2D(Camera);
        }

        public static void End()
        {
            RB.EndMode2D();
        }

        public static Vector2 ScreenToUI(Vector2 screenPos)
        {
            return RB.GetScreenToWorld2D(screenPos, Camera);
        }
    }
}