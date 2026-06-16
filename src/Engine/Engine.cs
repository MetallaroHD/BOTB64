using System.Diagnostics;

namespace BOTB64.Engine
{
    public static class Engine
    {
        public static double Elapsed() => Clock.Elapsed.TotalSeconds;

        private static Stopwatch Clock = new Stopwatch();

        public static void Initialize()
        {
            Clock.Start();
        }

        public static void Update()
        {
            float dt = (float)Clock.Elapsed.TotalSeconds;
            Clock.Restart();
        }
    }
}
