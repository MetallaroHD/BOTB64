using BOTB64.Engine;
using BOTB64.Engine.States;
using System.Numerics;
using RL = Raylib_cs;

namespace BOTB64.Graphics.UI
{
    public class MainMenuScreen : UIScreen
    {
        public MainMenuScreen()
        {
            var background = new Background
            {
                Bounds = new RL.Rectangle(0, 0, 1280, 720),
                Color = new RL.Color(30, 30, 40, 255)
            };

            var title = new Label
            {
                Position = new Vector2(490, 220),
                Text = "BOTB64",
                FontSize = 48,
                Color = RL.Color.White
            };

            var startButton = new Button
            {
                Bounds = new RL.Rectangle(540, 360, 200, 60),
                Text = "Start",
                OnClick = () => StateManager.ChangeState(new GameplayState())
            };

            AddElement(background);
            AddElement(title);
            AddElement(startButton);
        }
    }
}
