using BOTB64.Entities;
using BOTB64.Entities.Effects;

namespace BOTB64.Engine
{
    [Flags]
    public enum AuraType
    {
        None = 0,
        Character = 1 << 0,
        Tile = 1 << 1,
    }

    public static class AuraTriggerManager
    {
        private static Game Parent;

        public static void Init(Game parent)
        {
            Parent = parent;
        }

        public static void Execute(EffectContext ctx, EffectTrigger condition, AuraType type)
        {
            if (Parent == null)
                throw new ArgumentNullException("AuraTriggerManager not initialized!");

            Character invoker = ctx.Invoker;
            if(invoker == null)
                throw new ArgumentNullException("Invoker not set!");

            if (type.HasFlag(AuraType.Character))
            {
                foreach (var aura in invoker.Auras)
                    aura.Execute(Parent, ctx, condition);
            }
            if(type.HasFlag(AuraType.Tile))
            {
                Tile tile = Parent.GetBoard().GetTile(invoker.Position);
                if (tile != null)
                    foreach (var aura in tile.Effects)
                        aura.Execute(Parent, ctx, condition);
            }
        }
    }
}
