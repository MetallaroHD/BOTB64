using BOTB64.Engine.States;
using BOTB64.Graphics.G3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOTB64.Engine.Actions
{
    public class PauseAction : ActionBase
    {
        public PauseAction(GameplayState parent) : base(parent)
        {
        }

        public override void Enter()
        {
            ((GameplayState)Parent).ToggleCameraControl(false);
            ((GameplayState)Parent).TogglePauseOverlay(true);
        }

        public override void Exit()
        {
            ((GameplayState)Parent).TogglePauseOverlay(false);
            ((GameplayState)Parent).ToggleCameraControl(true);
        }

        public override void Update()
        {
        }
    }
}
