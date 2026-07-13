using BOTB64.Engine.States;
using BOTB64.Graphics.G3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOTB64.Engine.Actions
{
    public enum PauseMode
    {
        Esc = 0,
        Turn = 1,
    }

    public class PauseAction : ActionBase
    {
        public PauseMode Mode { get; set; }

        public PauseAction(GameplayState parent) : base(parent)
        {
        }

        public override void Enter()
        {
            ((GameplayState)Parent).ToggleCameraControl(false);
            switch(Mode)
            {
                case PauseMode.Esc:
                    ((GameplayState)Parent).TogglePauseOverlay(true);
                    break;
                case PauseMode.Turn:
                    ((GameplayState)Parent).ToggleAskEndTurn(true);
                    break;
            }
        }

        public override void Exit()
        {
            switch (Mode)
            {
                case PauseMode.Esc:
                    ((GameplayState)Parent).TogglePauseOverlay(false);
                    break;
                case PauseMode.Turn:
                    ((GameplayState)Parent).ToggleAskEndTurn(false);
                    break;
            }
            ((GameplayState)Parent).ToggleCameraControl(true);
        }

        public override void Update()
        {
        }
    }
}
