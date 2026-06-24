using BOTB64.Engine.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOTB64.Engine.Actions
{
    public class DefaultAction : IAction
    {
        private GameplayState Parent;

        public DefaultAction(GameplayState parent)
        { 
            this.Parent = parent; 
        }

        public void Enter()
        {

        }

        public void Exit()
        {

        }

        public void Update() 
        { 

        }
    }
}
