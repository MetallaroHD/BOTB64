using BOTB64.Engine.States;
using BOTB64.Entities;
using BOTB64.Runtime;

namespace BOTB64.Engine.Actions
{
    public class AutoAttackAction : TargetingAction
    {
        Character Character;

        public AutoAttackAction(GameplayState parent) : base(parent)
        {
        }

        public override void Enter()
        {
            CursorManager.SetCursor("Attack");
            if (Character == null)
            {
                throw new InvalidOperationException("Character must be set before entering CharacterMoveAction.");
            }
            Targeter.SetTargetingData(new TargetingData
            {
                Type = TargetingType.Direct,
                Source = Character.Position,
                Radius = Character.AutoAttackRange.GetI(),
            });
            Update();
        }

        public override void Update()
        {
            base.Update();
        }

        public void SetCurrentCharacter(Character character)
        {
            Character = character;
        }

        public Character? ConfirmTarget()
        {
            if (Targeter.Targeted.Count <= 0)
                return null;

            return Targeter.Targeted[0].Character;
        }
    }
}
