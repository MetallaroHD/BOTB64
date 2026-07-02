using BOTB64.Engine.States;
using BOTB64.Entities;
using BOTB64.Runtime;

namespace BOTB64.Engine.Actions
{
    public class CharacterMoveAction : TargetingAction
    {
        Character Character;

        public CharacterMoveAction(GameplayState parent) : base(parent)
        {
        }

        public override void Enter()
        {
            if(Character == null)
            {
                throw new InvalidOperationException("Character must be set before entering CharacterMoveAction.");
            }
            Targeter.SetTargetingData(new TargetingData
            {
                Type = TargetingType.Pathfinding,
                Radius = Character.RemainMovement,
                Source = Character.Position,
            });
            Update();
        }

        public override void Update()
        {
            Targeter.Data.Radius = Character.RemainMovement;
            Targeter.Data.Source = Character.Position;
            base.Update();
        }

        public void SetCurrentCharacter(Character character)
        {
            Character = character;
        }

        public List<Hex> GetPath()
        {
            List<Hex> ret = new();
            foreach (var tile in Targeter.Targeted)
                ret.Add(tile.AxialPosition);
            return ret;
        }

        public void CycleToNextPath()
        {
            Targeter.GetNextPathfinding();
        }

        public override void Exit()
        {
            Targeter.ResetPathfinding();
            base.Exit();
        }
    }
}
