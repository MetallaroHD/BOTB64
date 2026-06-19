using System.Diagnostics;

namespace BOTB64.Engine
{
    public static class Engine
    {
        private static TimeSpan CurrentTime;
        private static TimeSpan PreviousTime;
        private static Stopwatch Clock = new Stopwatch();

        public static float DeltaTime => (float)(CurrentTime - PreviousTime).TotalSeconds;


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

            Graphics.Graphics.EndFrame();
        }
    }
}
