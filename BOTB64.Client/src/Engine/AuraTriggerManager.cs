using BOTB64.Entities;
using BOTB64.Entities.DTOs;
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

        public static void ClearCache()
        {
            AuraTemplates.Clear();
            TileEffectTemplates.Clear();
        }

        public static void CacheAllAuras()
        {
            //for now it's fine like this, however at some point we may want to give spells a REQUIRE property to only cache necessary auras
            JsonDataFile<AuraDTO> af = new JsonDataFile<AuraDTO>();
            List<AuraDTO> auras = af.DeserializeAll(new DataFile(CommonURIs.AuraJSON));
            //create all auras

            JsonDataFile<TileEffectDTO> tf = new JsonDataFile<TileEffectDTO>();
            List<TileEffectDTO> tileffs = tf.DeserializeAll(new DataFile(CommonURIs.TileEffJSON));
            //create all teffs
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
