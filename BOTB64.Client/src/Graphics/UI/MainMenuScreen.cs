using BOTB64.Engine;
using BOTB64.Engine.States;
using BOTB64.Runtime;
using BOTB64.Shared;
using System.Numerics;
using RL = Raylib_cs;

namespace BOTB64.Graphics.UI
{
    public class MainMenuScreen : UIScreen
    {
        public MainMenuState Controller;

        Background Background = new Background { Bounds = new RL.Rectangle(0, 0, 1280, 720), Color = new RL.Color(30, 30, 40, 255) };
        Label Title = new Label { Position = new Vector2(546, 160), Text = "BOTB64", FontSize = 48, Color = RL.Color.White };
        TextButton StartButton = new TextButton { Bounds = new RL.Rectangle(540, 300, 200, 60), Text = "Start", };
        TextButton SettingsButton = new TextButton() { Bounds = new RL.Rectangle(540, 380, 200, 60), Text = "Settings", };
        TextButton ExitButton = new TextButton() { Bounds = new RL.Rectangle(540, 460, 200, 60), Text = "Exit", OnClick = () => { InputManager.WantsClose = true; } };
        TextButton BackButton = new TextButton() { Bounds = new RL.Rectangle(540, 540, 200, 60), Text = "Back", };
        TextButton LocalGameButton = new TextButton() { Bounds = new RL.Rectangle(540, 300, 200, 60), Text = "Local Play", };
        TextButton RandomOnlineButton = new TextButton() { Bounds = new RL.Rectangle(540, 380, 200, 60), Text = "Online Ranked", };
        TextButton CustomGameButton = new TextButton() { Bounds = new RL.Rectangle(540, 460, 200, 60), Text = "Custom Online", };
        TextButton V2Button = new TextButton() { Bounds = new RL.Rectangle(540, 300, 200, 60), Text = "2v2", };
        TextButton V3Button = new TextButton() { Bounds = new RL.Rectangle(540, 380, 200, 60), Text = "3v3", };
        TextButton V5Button = new TextButton() { Bounds = new RL.Rectangle(540, 460, 200, 60), Text = "5v5", };
        TextButton V2PButton = new TextButton() { Bounds = new RL.Rectangle(430, 300, 200, 60), Text = "2-Player 2v2", };
        TextButton V3PButton = new TextButton() { Bounds = new RL.Rectangle(430, 380, 200, 60), Text = "2-Player 3v3", };
        TextButton V5PButton = new TextButton() { Bounds = new RL.Rectangle(430, 460, 200, 60), Text = "2-Player 5v5", };
        TextButton V2TButton = new TextButton() { Bounds = new RL.Rectangle(650, 300, 200, 60), Text = "Team 2v2", };
        TextButton V3TButton = new TextButton() { Bounds = new RL.Rectangle(650, 380, 200, 60), Text = "Team 3v3", };
        TextButton V5TButton = new TextButton() { Bounds = new RL.Rectangle(650, 460, 200, 60), Text = "Team 5v5", };

        public MainMenuScreen()
        {
            AddElement(Background);
            AddElement(Title);
            AddElement(StartButton);
            AddElement(SettingsButton);
            AddElement(ExitButton);
            AddElement(BackButton);
            AddElement(LocalGameButton);
            AddElement(RandomOnlineButton);
            AddElement(CustomGameButton);
            AddElement(V2Button);
            AddElement(V3Button);
            AddElement(V5Button);
            AddElement(V2PButton);
            AddElement(V3PButton);
            AddElement(V5PButton);
            AddElement(V2TButton);
            AddElement(V3TButton);
            AddElement(V5TButton);
        }

        public override void Enter()
        {
            StartButton.OnClick = () => { MoveToGameSelect(); };
            BackButton.OnClick = () => { MoveToStart(); };
            LocalGameButton.OnClick = () => { Controller.SetChosenGT(GameType.Local); MoveToSizeSelectLocal(); };
            RandomOnlineButton.OnClick = () => { Controller.SetChosenGT(GameType.RandomRanked); MoveToSizeSelectRanked(); };
            CustomGameButton.OnClick = () => { Controller.SetChosenGT(GameType.IPMultiplayer); MoveToLobbySelect(); };
            V2Button.OnClick = () => { Controller.SetChosenST(GameSizeType.v2P); Controller.StartLocalGame(); };
            V3Button.OnClick = () => { Controller.SetChosenST(GameSizeType.v3P); Controller.StartLocalGame(); };
            V5Button.OnClick = () => { Controller.SetChosenST(GameSizeType.v5P); Controller.StartLocalGame(); };
            V2PButton.OnClick = () => { Controller.SetChosenST(GameSizeType.v2P); MoveToRankedMatchmaking(); };
            V3PButton.OnClick = () => { Controller.SetChosenST(GameSizeType.v3P); MoveToRankedMatchmaking(); };
            V5PButton.OnClick = () => { Controller.SetChosenST(GameSizeType.v5P); MoveToRankedMatchmaking(); };
            V2TButton.OnClick = () => { Controller.SetChosenST(GameSizeType.v2T); MoveToRankedMatchmaking(); };
            V3TButton.OnClick = () => { Controller.SetChosenST(GameSizeType.v3T); MoveToRankedMatchmaking(); };
            V5TButton.OnClick = () => { Controller.SetChosenST(GameSizeType.v5T); MoveToRankedMatchmaking(); };

            MoveToStart();
        }

        private void HideAll()
        {
            foreach (var element in Elements)
            {
                if (element is Button)
                    element.Visible = false;
            }
        }

        private void MoveToStart()
        {
            HideAll();
            StartButton.Visible = true;
            SettingsButton.Visible = true;
            ExitButton.Visible = true;
        }

        private void MoveToGameSelect()
        {
            HideAll();
            LocalGameButton.Visible = true;
            RandomOnlineButton.Visible = true;
            CustomGameButton.Visible = true;
            BackButton.Visible = true;
        }

        private void MoveToSizeSelectLocal()
        {
            HideAll();
            V2Button.Visible = true;
            V3Button.Visible = true;
            V5Button.Visible = true;
            BackButton.Visible = true;
        }

        private void MoveToSizeSelectRanked()
        {
            HideAll();
            V2PButton.Visible = true;
            V3PButton.Visible = true;
            V5PButton.Visible = true;
            V2TButton.Visible = true;
            V3TButton.Visible = true;
            V5TButton.Visible = true;
            BackButton.Visible = true;
        }

        private void MoveToRankedMatchmaking()
        {
            //TBI - make new gamestate
        }

        private void MoveToLobbySelect()
        {
            StateManager.ChangeState(new LobbyState());
        }
        private void MoveToSettings()
        {
            //TBI - make new gamestate
        }
    }
}
