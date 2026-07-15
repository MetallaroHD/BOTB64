using RL = Raylib_cs;

namespace BOTB64.Graphics.UI
{
    public static class FloatingMessageManager
    {
        static Label Message1;
        static Label Message2;
        static Label Message3;

        static UIScreen Screen;
        public static void Init(UIScreen screen)
        {
            Screen = screen;
        }

        public static void AddMessage(string message, int duration = 5, int fontSize = 10, RL.Color? color = null) 
        {
        }
    }
}
