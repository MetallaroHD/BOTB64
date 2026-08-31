using BOTB64.Runtime;
using System.Diagnostics;

namespace BOTB64.Engine
{
    public static class Engine
    {
        private static TimeSpan CurrentTime;
        private static TimeSpan PreviousTime;
        private static Stopwatch Clock = new Stopwatch();

        // Capped so a debugger pause (or a real multi-second hitch) can't produce a single
        // huge dt that instantly completes time-based effects (animations, VFX, etc.).
        private const float MaxDeltaTime = 0.1f;

        public static float DeltaTime => Math.Min((float)(CurrentTime - PreviousTime).TotalSeconds, MaxDeltaTime);

        public static void Initialize()
        {
            Clock.Start();
            StateManager.ChangeState(new States.MainMenuState());
        }

        public static void Update()
        {
            PreviousTime = CurrentTime;
            CurrentTime = Clock.Elapsed;

            StateManager.FlushPendingState();
            StateManager.Update(DeltaTime);
        }

        public static void Render()
        {
            Graphics.Graphics.BeginFrame();

            StateManager.Render();
            CursorManager.Draw();

            Graphics.Graphics.EndFrame();
        }
    }
}
