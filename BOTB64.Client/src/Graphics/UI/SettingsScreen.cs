using BOTB64.Engine.States;
using BOTB64.Runtime;
using System.Globalization;
using System.Numerics;
using RL = Raylib_cs;

namespace BOTB64.Graphics.UI
{
    public class SettingsScreen : UIScreen
    {
        public SettingsState Controller;

        Background Background = new Background { Bounds = new RL.Rectangle(0, 0, 1280, 720), Color = new RL.Color(30, 30, 40, 255) };
        Label Title = new Label { Position = new Vector2(520, 100), Text = "Settings", FontSize = 40, Color = RL.Color.White };

        Label ScaleLabel = new Label { Position = new Vector2(400, 225), Text = "UI Scale:", FontSize = 20, Color = RL.Color.White };
        TextBox ScaleTextBox = new TextBox { Bounds = new RL.Rectangle(600, 210, 150, 40), NumericOnly = true, MaxLength = 6 };
        Label ScaleErrorLabel = new Label { Position = new Vector2(770, 225), Text = "", FontSize = 16, Color = RL.Color.Red };

        Label FullScreenLabel = new Label { Position = new Vector2(400, 295), Text = "Fullscreen:", FontSize = 20, Color = RL.Color.White };
        TextButton FullScreenButton = new TextButton { Bounds = new RL.Rectangle(600, 280, 120, 40), Text = "Off" };

        Label VSyncLabel = new Label { Position = new Vector2(400, 365), Text = "VSync:", FontSize = 20, Color = RL.Color.White };
        TextButton VSyncButton = new TextButton { Bounds = new RL.Rectangle(600, 350, 120, 40), Text = "On" };

        Label AskEndTurnLabel = new Label { Position = new Vector2(400, 435), Text = "Ask End Turn:", FontSize = 20, Color = RL.Color.White };
        TextButton AskEndTurnButton = new TextButton { Bounds = new RL.Rectangle(600, 420, 120, 40), Text = "On" };

        TextButton SaveButton = new TextButton { Bounds = new RL.Rectangle(460, 560, 150, 60), Text = "Save" };
        TextButton BackButton = new TextButton { Bounds = new RL.Rectangle(650, 560, 150, 60), Text = "Back" };

        private bool _pendingFullScreen;
        private bool _pendingVSync;
        private bool _pendingAskEndTurn;

        public SettingsScreen()
        {
            AddElement(Background);
            AddElement(Title);
            AddElement(ScaleLabel);
            AddElement(ScaleTextBox);
            AddElement(ScaleErrorLabel);
            AddElement(FullScreenLabel);
            AddElement(FullScreenButton);
            AddElement(VSyncLabel);
            AddElement(VSyncButton);
            AddElement(AskEndTurnLabel);
            AddElement(AskEndTurnButton);
            AddElement(SaveButton);
            AddElement(BackButton);
        }

        public override void Enter()
        {
            // pull current settings in as the "pending" (uncommitted) state
            ScaleTextBox.Text = Settings.Scale.ToString(CultureInfo.InvariantCulture);
            ScaleErrorLabel.Text = "";

            _pendingFullScreen = Settings.FullScreen;
            _pendingVSync = Settings.VSync;
            _pendingAskEndTurn = Settings.AskEndTurn;

            SetToggleText(FullScreenButton, _pendingFullScreen);
            SetToggleText(VSyncButton, _pendingVSync);
            SetToggleText(AskEndTurnButton, _pendingAskEndTurn);

            FullScreenButton.OnClick = () => { _pendingFullScreen = !_pendingFullScreen; SetToggleText(FullScreenButton, _pendingFullScreen); };
            VSyncButton.OnClick = () => { _pendingVSync = !_pendingVSync; SetToggleText(VSyncButton, _pendingVSync); };
            AskEndTurnButton.OnClick = () => { _pendingAskEndTurn = !_pendingAskEndTurn; SetToggleText(AskEndTurnButton, _pendingAskEndTurn); };

            SaveButton.OnClick = () => { Save(); };
            BackButton.OnClick = () => { Controller.MoveToMainMenu(); };
        }

        private static void SetToggleText(TextButton button, bool state)
        {
            button.Text = state ? "On" : "Off";
        }

        private void Save()
        {
            // validate the scale textbox as a culture-invariant float before doing anything else
            if (!float.TryParse(ScaleTextBox.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out float scale) || scale <= 0f)
            {
                ScaleErrorLabel.Text = "Invalid scale";
                return;
            }
            ScaleErrorLabel.Text = "";

            Settings.FullScreen = _pendingFullScreen;
            Settings.AskEndTurn = _pendingAskEndTurn;

            // apply live, on the spot, before writing to disk
            BOTB64.Graphics.Graphics.ApplyScale(scale);
            BOTB64.Graphics.Graphics.ApplyVSync(_pendingVSync);

            Settings.Save();
        }
    }
}