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

            EffectContext ctx = new EffectContext
            {
                Targets = [ActiveCharacter],
                Game = Parent,
                TargetTiles = [Parent.GetBoard().GetTile(ActiveCharacter.Position)]
            };

            foreach (var aura in ActiveCharacter.Auras) 
            {
                foreach (var effect in aura.Effects)
                {
                    if (effect.Trigger.HasFlag(Effects.EffectTrigger.OnStartTurn))
                    {
                        ctx.Caster = aura.Owner;
                        effect.Execute(ctx);
                    }
                }
            }

            foreach (var aura in Parent.GetBoard().GetTile(ActiveCharacter.Position).Effects)
            {
                foreach (var effect in aura.Effects)
                {
                    if (effect.Trigger.HasFlag(Effects.EffectTrigger.OnStartTurn))
                    {
                        ctx.Caster = aura.Owner;
                        effect.Execute(ctx);
                    }
                }
            }
        }

        public void End()
        {
            if (!ActiveCharacter.Alive)
                return;

            EffectContext ctx = new EffectContext
            {
                Targets = [ActiveCharacter],
                Game = Parent,
                TargetTiles = [Parent.GetBoard().GetTile(ActiveCharacter.Position)]
            };

            foreach (var aura in ActiveCharacter.Auras)
            {
                foreach (var effect in aura.Effects)
                {
                    if (effect.Trigger.HasFlag(Effects.EffectTrigger.OnEndTurn))
                    {
                        ctx.Caster = aura.Owner;
                        effect.Execute(ctx);
                    }
                }
            }

            foreach (var aura in Parent.GetBoard().GetTile(ActiveCharacter.Position).Effects)
            {
                foreach (var effect in aura.Effects)
                {
                    if (effect.Trigger.HasFlag(Effects.EffectTrigger.OnEndTurn))
                    {
                        ctx.Caster = aura.Owner;
                        effect.Execute(ctx);
                    }
                }
            }
        }
    }
}
