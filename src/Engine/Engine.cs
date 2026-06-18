using System.Diagnostics;
using System.Runtime.Versioning;

namespace BOTB64.Engine
{
    public static class Engine
    {
        private static TimeSpan CurrentTime;
        private static TimeSpan PreviousTime;

        public static float Elapsed() => (float) (CurrentTime - PreviousTime).TotalSeconds;

        private static Stopwatch Clock = new Stopwatch();

        public static void Initialize()
        {
            Clock.Start();
        }

        public static void Update()
        {
            PreviousTime = CurrentTime;
            CurrentTime = Clock.Elapsed;

            //rest if the update
        }
    }
}
