using BOTB64.Entities.Effects;

namespace BOTB64.Entities
{
    public interface IExecutable
    {
        void Execute(Game game, EffectContext ctx, EffectTrigger cond);
    }
}
