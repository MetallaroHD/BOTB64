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

            foreach (var aura in ActiveCharacter.Auras)
                aura.Execute(Parent, new EffectContext(), EffectTrigger.OnStartTurn);

            foreach (var aura in Parent.GetBoard().GetTile(ActiveCharacter.Position).Effects)
                aura.Execute(Parent, new EffectContext(), EffectTrigger.OnStartTurn);
        }

        public void End()
        {
            if (!ActiveCharacter.Alive)
                return;

            foreach (var aura in ActiveCharacter.Auras)
                aura.Execute(Parent, new EffectContext(), EffectTrigger.OnEndTurn);

            foreach (var aura in Parent.GetBoard().GetTile(ActiveCharacter.Position).Effects)
                aura.Execute(Parent, new EffectContext(), EffectTrigger.OnEndTurn);
        }
    }
}
