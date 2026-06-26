using BOTB64.Entities.Effects;

namespace BOTB64.Entities
{
    public class ExecutableBase : IExecutable
    {
        protected List<Effect> Effects = new();

        public void Execute(Game game, EffectContext ctx, EffectTrigger trigger)
        {
            foreach (var effect in Effects)
            {
                if (effect.Trigger.HasFlag(trigger))
                {
                    effect.Execute(ctx);
                }
            }
        }
    }
}
