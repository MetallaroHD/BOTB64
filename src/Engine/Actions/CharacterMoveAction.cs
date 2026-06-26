using BOTB64.Engine.States;
using BOTB64.Entities;

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
            //reinit pathfinder(?)
            Targeter.SetTargetingData(new TargetingData
            {
                Type = TargetingType.BeamNoLos, //change to pathfinding
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

        public List<Tile> GetPath()
        {
            return Targeter.Targeted;
        }
    }
}
