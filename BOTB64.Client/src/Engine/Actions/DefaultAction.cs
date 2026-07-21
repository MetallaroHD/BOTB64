using BOTB64.Engine.States;
using BOTB64.Entities;
using BOTB64.Runtime;

namespace BOTB64.Engine.Actions
{
    public class DefaultAction : TargetingAction
    {
        public DefaultAction(GameplayState parent) : base(parent)
        {
        }

        public override void Enter()
        {
            CursorManager.SetCursor("Idle");
            Targeter.SetTargetingData(new TargetingData
            {
                Type = TargetingType.Direct,
                Secret = false,
                Radius = HexAlgo.MaxCircleRadius,
                Source = new Hex(0, 0)
            });
            Update();
            base.Enter();
        }

        public override void Update()
        {
            base.Update();
        }

        public Character? GetTarget()
        {
            if (Targeter.Targeted.Count <= 0)
                return null;

            return Targeter.Targeted[0].Character;
        }
    }
}
