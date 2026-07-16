using RL = Raylib_cs;

namespace BOTB64.Graphics.UI
{
    public static class FloatingMessageManager
    {
        private static UIScreen Screen;
        private static readonly List<TimedLabel> Labels = new();

        public static void Init(UIScreen screen)
        {
            Screen = screen;
        }

        public static void AddMessage(string message, float duration = 5f, int size = 24, RL.Color? color = null)
        {
            var label = new TimedLabel();
            label.Setup(message, duration, size, color ?? RL.Color.Red);

            Labels.Add(label);
            Screen.AddElement(label);
        }

        public static void Update(float dt)
        {
            for (int i = Labels.Count - 1; i >= 0; i--)
            {
                if (Labels[i].Finished)
                {
                    Screen.RemoveElement(Labels[i]);
                    Labels.RemoveAt(i);
                }
            }
        }
    }
}