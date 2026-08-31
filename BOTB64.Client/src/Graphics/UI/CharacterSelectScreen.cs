using BOTB64.Engine;
using BOTB64.Engine.States;
using Raylib_cs;
using System.Numerics;
using RL = Raylib_cs;

namespace BOTB64.Graphics.UI
{
    public class CharacterSelectScreen : UIScreen
    {
        public TextButton LockButton;
        public TextButton StartButton;
        public Label NowPickingLabel;
        public TeamStripDisplay BlueStrip;
        public TeamStripDisplay RedStrip;
        public ModelPreviewPanel CharacterPreview;
        public Label CharacterNameLabel;
        public TextButton BackButton;

        public CharacterSelectScreen()
        {
            var background = new Background
            {
                Bounds = new RL.Rectangle(0, 0, 880, 720),
                Color = new RL.Color(30, 30, 40, 255)
            };

            NowPickingLabel = new Label
            {
                Position = new Vector2(20, 600),
                Text = "",
                FontSize = 22,
                Color = RL.Color.White
            };

            BlueStrip = new TeamStripDisplay
            {
                Bounds = new RL.Rectangle(0, 630, 640, 90),
                BackgroundColor = new RL.Color(0x30, 0x50, 0x90, 0xFF)
            };
            RedStrip = new TeamStripDisplay
            {
                Bounds = new RL.Rectangle(640, 630, 640, 90),
                BackgroundColor = new RL.Color(0x90, 0x30, 0x30, 0xFF)
            };
            CharacterPreview = new ModelPreviewPanel(400, 440)
            {
                Bounds = new RL.Rectangle(880, 0, 400, 480)
            };

            CharacterNameLabel = new Label
            {
                Position = new Vector2(885, 500),
                Text = "",
                FontSize = 26,
                Color = RL.Color.Black
            };

            LockButton = new TextButton { Bounds = new RL.Rectangle(880, 530, 200, 100), Text = "Lock in" };
            StartButton = new TextButton { Bounds = new RL.Rectangle(1080, 530, 200, 100), Text = "Start" };

            // Top-right corner, on top of CharacterPreview's viewport (which spans
            // x:880-1280, y:0-480) - added to Elements after it so it draws over it.
            BackButton = new TextButton { Bounds = new RL.Rectangle(1240, 5, 35, 35), Text = "<" };

            AddElement(background);
            AddElement(BlueStrip);
            AddElement(RedStrip);
            AddElement(NowPickingLabel);
            AddElement(StartButton);
            AddElement(LockButton);
            AddElement(CharacterPreview);
            AddElement(CharacterNameLabel);
            AddElement(BackButton);
        }

        public override void Draw()
        {
            CharacterPreview.RenderToTexture();
            base.Draw();
        }
    }
}