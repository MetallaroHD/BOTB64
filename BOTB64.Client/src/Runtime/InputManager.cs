using RL = Raylib_cs;
using System.Numerics;
using RB = Raylib_cs.Raylib;

namespace BOTB64.Runtime
{
    public static class InputManager
    {
        public static bool WantsClose = false;
        private static bool ClickUsed = false;
        public static bool IsMouseButtonPressed(RL.MouseButton btn) => RB.IsMouseButtonPressed(btn);
        public static bool IsMouseButtonDown(RL.MouseButton btn) => RB.IsMouseButtonDown(btn);
        public static bool IsKeyDown(RL.KeyboardKey key) => RB.IsKeyDown(key);
        public static bool IsKeyPressed(RL.KeyboardKey key) => RB.IsKeyPressed(key);
        public static bool IsKeyReleased(RL.KeyboardKey key) => RB.IsKeyReleased(key);
        public static float ScrollDelta => RB.GetMouseWheelMove();
        public static Vector2 MouseDelta => RB.GetMouseDelta();
        public static Vector2 MousePosition => RB.GetMousePosition();
        public static bool IsLMP => !ClickUsed && IsMouseButtonPressed(RL.MouseButton.Left);
        public static bool IsRMP => IsMouseButtonPressed(RL.MouseButton.Right);
        public static bool IsLMD => IsMouseButtonDown(RL.MouseButton.Left);
        public static bool IsRMD => IsMouseButtonDown(RL.MouseButton.Right);

        public static void UseClick() => ClickUsed = true;
        public static void NewFrame() => ClickUsed = false;

        public static bool ShouldClose()
        {
            if(WantsClose) return true;

            return RB.WindowShouldClose();
        }
    }
}
