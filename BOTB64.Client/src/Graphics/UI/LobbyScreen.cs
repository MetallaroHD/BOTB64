using BOTB64.Engine.States;
using BOTB64.Runtime;
using BOTB64.Shared;
using System.Numerics;
using RL = Raylib_cs;

namespace BOTB64.Graphics.UI
{
    public class LobbyScreen : UIScreen
    {
        public LobbyState Controller;

        Background Background = new Background { Bounds = new RL.Rectangle(0, 0, 1280, 720), Color = new RL.Color(30, 30, 40, 255) };
        Label Title = new Label { Position = new Vector2(500, 100), Text = "Custom Online", FontSize = 40, Color = RL.Color.White };

        // --- Setup page ---
        Label NameLabel = new Label { Position = new Vector2(440, 200), Text = "Display Name", FontSize = 18, Color = RL.Color.LightGray };
        TextBox NameBox = new TextBox { Bounds = new RL.Rectangle(440, 225, 400, 40), Placeholder = "Enter your name", MaxLength = 20 };

        Label AddressLabel = new Label { Position = new Vector2(440, 280), Text = "Server Address", FontSize = 18, Color = RL.Color.LightGray };
        TextBox AddressBox = new TextBox { Bounds = new RL.Rectangle(440, 305, 400, 40), Placeholder = "e.g. localhost:5000" };

        Label LobbyIdLabel = new Label { Position = new Vector2(440, 360), Text = "Lobby ID (leave blank to create)", FontSize = 18, Color = RL.Color.LightGray };
        TextBox LobbyIdBox = new TextBox { Bounds = new RL.Rectangle(440, 385, 400, 40), Placeholder = "Join code" };

        TextButton ConnectButton = new TextButton { Bounds = new RL.Rectangle(440, 450, 190, 50), Text = "Connect" };
        TextButton CreateButton = new TextButton { Bounds = new RL.Rectangle(650, 450, 190, 50), Text = "Create Lobby" };
        TextButton SetupBackButton = new TextButton { Bounds = new RL.Rectangle(440, 520, 400, 50), Text = "Back" };

        Label StatusLabel = new Label { Position = new Vector2(440, 580), Text = "", FontSize = 16, Color = RL.Color.Red };

        // --- Waiting room page ---
        Label LobbyIdShareLabel = new Label { Position = new Vector2(440, 150), Text = "", FontSize = 20, Color = RL.Color.Gold };
        Label ModeLabel = new Label { Position = new Vector2(440, 180), Text = "", FontSize = 18, Color = RL.Color.LightGray };

        List<Label> PlayerLabels = new(); // populated dynamically, see RefreshPlayerList

        TextButton ModeV2PButton = new TextButton { Bounds = new RL.Rectangle(440, 420, 130, 40), Text = "2P" };
        TextButton ModeV3PButton = new TextButton { Bounds = new RL.Rectangle(580, 420, 130, 40), Text = "3P" };
        TextButton ModeV5PButton = new TextButton { Bounds = new RL.Rectangle(720, 420, 130, 40), Text = "5P" };
        TextButton ModeV2TButton = new TextButton { Bounds = new RL.Rectangle(440, 470, 130, 40), Text = "2v2 Team" };
        TextButton ModeV3TButton = new TextButton { Bounds = new RL.Rectangle(580, 470, 130, 40), Text = "3v3 Team" };
        TextButton ModeV5TButton = new TextButton { Bounds = new RL.Rectangle(720, 470, 130, 40), Text = "5v5 Team" };

        TextButton StartButton = new TextButton { Bounds = new RL.Rectangle(440, 540, 410, 50), Text = "Start Game" };
        TextButton WaitingBackButton = new TextButton { Bounds = new RL.Rectangle(440, 600, 410, 50), Text = "Leave Lobby" };

        private readonly List<IUIElement> _setupElements;
        private readonly List<IUIElement> _waitingElements;

        public LobbyScreen()
        {
            _setupElements = new List<IUIElement>
            {
                NameLabel, NameBox, AddressLabel, AddressBox, LobbyIdLabel, LobbyIdBox,
                ConnectButton, CreateButton, SetupBackButton, StatusLabel
            };
            _waitingElements = new List<IUIElement>
            {
                LobbyIdShareLabel, ModeLabel,
                ModeV2PButton, ModeV3PButton, ModeV5PButton, ModeV2TButton, ModeV3TButton, ModeV5TButton,
                StartButton, WaitingBackButton
            };

            AddElement(Background);
            AddElement(Title);
            foreach (var e in _setupElements) AddElement(e);
            foreach (var e in _waitingElements) AddElement(e);
        }

        public override void Enter()
        {
            ConnectButton.OnClick = () =>
            {
                if (string.IsNullOrWhiteSpace(LobbyIdBox.Text)) { StatusLabel.Text = "Enter a Lobby ID to connect, or use Create Lobby."; return; }
                Controller.OnConnectClicked(NameBox.Text, AddressBox.Text, LobbyIdBox.Text);
            };
            CreateButton.OnClick = () => Controller.OnCreateClicked(NameBox.Text, AddressBox.Text);
            SetupBackButton.OnClick = () => Controller.OnBackFromSetup();

            ModeV2PButton.OnClick = () => Controller.OnModeSelected(GameSizeType.V2P);
            ModeV3PButton.OnClick = () => Controller.OnModeSelected(GameSizeType.V3P);
            ModeV5PButton.OnClick = () => Controller.OnModeSelected(GameSizeType.V5P);
            ModeV2TButton.OnClick = () => Controller.OnModeSelected(GameSizeType.V2T);
            ModeV3TButton.OnClick = () => Controller.OnModeSelected(GameSizeType.V3T);
            ModeV5TButton.OnClick = () => Controller.OnModeSelected(GameSizeType.V5T);

            StartButton.OnClick = () => Controller.OnStartClicked();
            WaitingBackButton.OnClick = () => Controller.OnLeaveClicked();

            ShowSetupPage();
            LoadRecents();
        }

        public void ShowSetupPage()
        {
            foreach (var e in _setupElements) e.Visible = true;
            foreach (var e in _waitingElements) e.Visible = false;
            StatusLabel.Text = "";
        }

        public void ShowWaitingRoom(string joinCode, GameSizeType mode, bool isHost)
        {
            foreach (var e in _setupElements) e.Visible = false;
            foreach (var e in _waitingElements) e.Visible = true;

            LobbyIdShareLabel.Text = $"Lobby ID: {joinCode}";
            ModeLabel.Text = $"Mode: {mode}";

            bool showModeButtons = isHost;
            ModeV2PButton.Visible = showModeButtons;
            ModeV3PButton.Visible = showModeButtons;
            ModeV5PButton.Visible = showModeButtons;
            ModeV2TButton.Visible = showModeButtons;
            ModeV3TButton.Visible = showModeButtons;
            ModeV5TButton.Visible = showModeButtons;
            StartButton.Visible = isHost;
        }

        public void SetStatus(string message) => StatusLabel.Text = message;

        public void RefreshPlayerList(List<string> names)
        {
            foreach (var label in PlayerLabels)
                Elements.Remove(label);

            PlayerLabels.Clear();

            float leftX = 440;
            float rightX = 700;
            float startY = 230;
            float spacingY = 30;

            for (int i = 0; i < names.Count; i++)
            {
                bool left = i % 2 == 0;

                var label = new Label
                {
                    Position = new Vector2(left ? leftX : rightX,
                                           startY + (i / 2) * spacingY),
                    Text = names[i],
                    FontSize = 18,
                    Color = RL.Color.White
                };

                PlayerLabels.Add(label);
                AddElement(label);
            }
        }
        private void LoadRecents()
        {
            //add actual recents later
            AddressBox.Text = "localhost:5000";
        }
    }
}