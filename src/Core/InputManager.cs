using Raylib_cs;
using System.Numerics;
using RB = Raylib_cs.Raylib;

namespace BOTB64.Core
{
    public static class InputManager
    {
        public static bool IsMouseButtonPressed(MouseButton btn) => RB.IsMouseButtonPressed(btn);
        public static bool IsMouseButtonDown(MouseButton btn) => RB.IsMouseButtonDown(btn);
        public static bool IsKeyDown(KeyboardKey key) => RB.IsKeyDown(key);
        public static bool IsKeyPressed(KeyboardKey key) => RB.IsKeyPressed(key);
        public static bool IsKeyReleased(KeyboardKey key) => RB.IsKeyReleased(key);
        public static float ScrollDelta => RB.GetMouseWheelMove();
        public static Vector2 MouseDelta => RB.GetMouseDelta();
        public static Vector2 MousePosition => RB.GetMousePosition();

        public static bool ShouldClose()
        {
            //and other conditions

            return RB.WindowShouldClose();
        }
    }
}
