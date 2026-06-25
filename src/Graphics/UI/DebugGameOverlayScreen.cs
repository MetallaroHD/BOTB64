using BOTB64.Engine;
using BOTB64.Engine.States;
using System.Numerics;
using RL = Raylib_cs;

namespace BOTB64.Graphics.UI
{
    public class DebugGameOverlayScreen : UIScreen
    {
        public Label PosLabel = new Label { Position = new Vector2(0, 0), FontSize = 36, Color = RL.Color.Black };

        public Label FPSLabel = new Label { Position = new Vector2(0, 36), FontSize = 36, Color = RL.Color.Black };

        public TextButton ButtonM = new TextButton {Text = "M" , Bounds = new RL.Rectangle(1180,620, 100,100)};
        public DebugGameOverlayScreen()
        {
            AddElement(FPSLabel);
            AddElement(PosLabel);
            AddElement(ButtonM);
        }
    }
}
