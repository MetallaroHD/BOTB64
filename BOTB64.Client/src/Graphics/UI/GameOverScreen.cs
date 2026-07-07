using BOTB64.Entities;
using System.Numerics;
using RL = Raylib_cs;

namespace BOTB64.Graphics.UI
{
    public class GameOverScreen : UIScreen
    {
        public TextButton MainMenuButton;
        private Label WinnerLabel;

        Background Background = new Background
        {
            Bounds = new RL.Rectangle(0, 0, 1280, 720),
            Color = new RL.Color(30, 30, 40, 255)
        };

        public GameOverScreen()
        {
            WinnerLabel = new Label
            {
                Position = new Vector2(1280 / 2f - 220, 300),
                Text = "",
                FontSize = 48,
                Color = RL.Color.White
            };

            MainMenuButton = new TextButton
            {
                Bounds = new RL.Rectangle(1280 / 2f - 120, 450, 240, 60),
                Text = "Main Menu"
            };

            AddElement(Background);
            AddElement(WinnerLabel);
            AddElement(MainMenuButton);
        }

        public void SetWinner(Faction winner)
        {
            string text = winner switch
            {
                Faction.BlueTeam => "Blue team wins!",
                Faction.RedTeam => "Red team wins!",
                _ => "It's a draw!"
            };

            WinnerLabel.Text = text;
            WinnerLabel.Color = winner switch
            {
                Faction.BlueTeam => new RL.Color(0x64, 0x8F, 0xFF, 0xFF),
                Faction.RedTeam => new RL.Color(0xFF, 0x61, 0x00, 0xFF),
                _ => RL.Color.White
            };
        }
    }
}