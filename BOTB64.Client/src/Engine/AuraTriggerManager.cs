using BOTB64.Entities;
using BOTB64.Shared.DTOs;
using BOTB64.Runtime;
using RL = Raylib_cs;

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

        private static List<Aura> AuraTemplates = new List<Aura>();
        private static List<TileEffect> TileEffectTemplates = new List<TileEffect>();

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
                foreach (var aura in invoker.CurrentAuras)
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

        public static Aura GetAura(int id)
        {
            // If it is already cached in the list we create a copy and give it to the character, otherwise first read the file and add it to the cache 
            return new Aura();
        }

        public static TileEffect GetTileEffect(int id)
        {
            // same
            return new TileEffect();
        }

        public static void ClearCache()
        {
            AuraTemplates.Clear();
            TileEffectTemplates.Clear();
        }

        public static RL.Texture2D? GetAuraIcon(int id)
        {
            foreach (var aura in AuraTemplates)
            {
                if(id == aura.ID)
                    return aura.Icon;
            }
            return null;
        }
    }
}
