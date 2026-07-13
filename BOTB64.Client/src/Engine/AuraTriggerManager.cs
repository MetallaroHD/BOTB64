using BOTB64.Entities;
using BOTB64.Entities.DTOs;
using BOTB64.Runtime;

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

        private static List<AuraDTO> CachedAuras = new List<AuraDTO>();

        public static void Init(Game parent)
        {
            Parent = parent;
            CacheAllAuras();
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

        public static void ClearCache()
        {
            CachedAuras.Clear();
        }

        public static void CacheAllAuras()
        {
            JsonDataFile<AuraDTO> af = new JsonDataFile<AuraDTO>();
            CachedAuras = af.DeserializeAll(new DataFile(CommonURIs.AuraJSON));
        }

        public static string GetAuraIcon(int id)
        {
            foreach (var aura in CachedAuras)
            {
                if(id == aura.ID)
                    return CommonURIs.GetAuraIcon(aura);
            }
            return "";
        }
    }
}
