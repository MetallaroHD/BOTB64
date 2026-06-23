using BOTB64.Engine;
using BOTB64.Engine.States;
using System.Numerics;
using RL = Raylib_cs;

namespace BOTB64.Graphics.UI
{
    public class CharacterSelectScreen : UIScreen
    {
        public TextButton LockButton;
        public TextButton StartButton;

        public CharacterSelectScreen()
        {
            var background = new Background
            {
                Bounds = new RL.Rectangle(0, 0, 1280, 720),
                Color = new RL.Color(30, 30, 40, 255)
            };

            LockButton = new TextButton
            {
                Bounds = new RL.Rectangle(880, 620, 200, 100),
                Text = "Lock in",
            };

            StartButton = new TextButton
            {
                Bounds = new RL.Rectangle(1080, 620, 200, 100),
                Text = "Start",
            };

            AddElement(background);
            AddElement(StartButton);
            AddElement(LockButton);
        }
    }
}
