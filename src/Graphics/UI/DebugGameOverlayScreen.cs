using BOTB64.Engine;
using BOTB64.Engine.States;
using System.Numerics;
using RL = Raylib_cs;

namespace BOTB64.Graphics.UI
{
    public class DebugGameOverlayScreen : UIScreen
    {
        public Label FPSLabel = new Label { Position = new Vector2(0, 0), FontSize = 36, Color = RL.Color.Black };

        public DebugGameOverlayScreen()
        {
            var savebutton = new Button
            {
                Bounds = new RL.Rectangle(1080, 660, 200, 60),
                Text = "Push",
                OnClick = () => StateManager.ChangeState(new GameplayState())
            };

            AddElement(FPSLabel);
            AddElement(savebutton);
        }
    }
}
