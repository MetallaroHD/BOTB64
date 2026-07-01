using RL = Raylib_cs;

namespace BOTB64.Graphics.UI
{
    public class GameOverlayScreen : UIScreen
    {
        public LogArea Log = new LogArea { Bounds = new RL.Rectangle(960, 560, 320, 160) };
        public GameOverlayScreen()
        {
            AddElement(Log);
        }
    }
}
