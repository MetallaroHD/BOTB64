namespace BOTB64.Entities
{
    public class ExecutableBase : IExecutable
    {
        protected List<Effect> Effects = new();
        public void AddEffect(Effect effect) => Effects.Add(effect);
        public void Execute(Game game, EffectContext ctx, EffectTrigger trigger)
        {
            foreach (var effect in Effects)
            {
                if (effect.Trigger.HasFlag(trigger))
                {
                    var runner = game.GetLuaRunner();
                    runner.Run(effect, ctx);
                }
            }
        }
    }
}
