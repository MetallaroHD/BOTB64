using BOTB64.Engine;
using BOTB64.Entities.Effects;

namespace BOTB64.Entities
{
    public class Turn
    {
        private Game Parent;

        public int Number { get; set; }
        public Character ActiveCharacter { get; set; }
        public bool HasMoved { get; set; }

        public Turn(int number, Character character, Game parent)
        {
            Parent = parent;
            Number = number;
            ActiveCharacter = character;
        }

        public void Begin()
        {
            if (!ActiveCharacter.Alive)
                return;

            ActiveCharacter.RemainMovement = ActiveCharacter.Speed;
            ActiveCharacter.RemainAction = 1;
            ActiveCharacter.RemainFastAction = 1;

            AuraTriggerManager.Execute(new EffectContext(ActiveCharacter), EffectTrigger.OnStartTurn, AuraType.Character | AuraType.Tile);
        }

        public void End()
        {
            if (!ActiveCharacter.Alive)
                return;

            AuraTriggerManager.Execute(new EffectContext(ActiveCharacter), EffectTrigger.OnStartTurn, AuraType.Character | AuraType.Tile);
        }
    }
}
