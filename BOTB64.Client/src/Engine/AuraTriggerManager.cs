using BOTB64.Entities;
using BOTB64.Shared.DTOs;
using BOTB64.Runtime;
using RL = Raylib_cs;
using BOTB64.Shared.Files;
using BOTB64.Graphics.G3D;

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

        // Triggers currently being processed on the call stack. Guards against an effect
        // running from a trigger (e.g. bonus damage on OnDamageDone) re-firing that same
        // trigger and recursing without bound - whether it's the same effect re-entering
        // itself or two separate effects chaining off each other's reactions.
        private static readonly HashSet<EffectTrigger> ActiveTriggers = new();

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

            if (!ActiveTriggers.Add(condition))
            {
                Logger.Log($"AuraTriggerManager: blocked a re-entrant {condition} trigger (an effect running from this trigger tried to fire it again).");
                return;
            }

            try
            {
                if (type.HasFlag(AuraType.Character))
                {
                    // Snapshot - a triggered effect may itself apply/drop an aura on this
                    // character, which would otherwise mutate CurrentAuras mid-iteration.
                    foreach (var aura in invoker.CurrentAuras.ToList())
                        aura.Execute(Parent, ctx, condition);
                }
                if(type.HasFlag(AuraType.Tile))
                {
                    Tile tile = Parent.GetBoard().GetTile(invoker.Position);
                    if (tile != null)
                        foreach (var aura in tile.Effects.ToList())
                            aura.Execute(Parent, ctx, condition);
                }
            }
            finally
            {
                ActiveTriggers.Remove(condition);
            }
        }

        public static Spell GetSpell(int id)
        {
            // No caching (for now)
            SpellDTO? spellD = DatabaseFileManager.Spells.FirstOrDefault(s => s.ID == id);

            if (spellD == null)
                throw new InvalidDataException("Spell not found!");

            SpellDataFile reader = new SpellDataFile();
            DataFile file = new DataFile(CommonURIs.GetSpellScript(spellD));
            Spell ret = reader.Read(file);

            ret.ID = spellD.ID;
            ret.Name = spellD.Name;
            ret.Icon = ResourceManager.GetSpellIcon(spellD.ID);

            return ret.Instance();
        }

        public static Aura GetAura(int id)
        {
            Aura? aura = AuraTemplates.FirstOrDefault(a => a.ID == id);

            if (aura != null)
                return aura.Instance();

            AuraDTO? auraD = DatabaseFileManager.Auras.FirstOrDefault(a => a.ID == id);

            if (auraD == null)
                throw new InvalidDataException("Aura not found!");

            AuraDataFile reader = new AuraDataFile();
            DataFile file = new DataFile(CommonURIs.GetAuraScript(auraD));
            aura = reader.Read(file);

            aura.ID = auraD.ID;
            aura.Name = auraD.Name;
            aura.Icon = ResourceManager.GetSpellIcon(aura.ID);
            aura.VfxID = auraD.AnimationURI ?? "";

            AuraTemplates.Add(aura);
            return aura.Instance();
        }

        public static TileEffect GetTileEffect(int id)
        {
            TileEffect? teff = TileEffectTemplates.FirstOrDefault(a => a.ID == id);

            if (teff != null)
                return teff.Instance();

            TileEffectDTO? tileD = DatabaseFileManager.TileEffects.FirstOrDefault(a => a.ID == id);

            if (tileD == null)
                throw new InvalidDataException("Tileeffect not found!");

            TileEffectDataFile reader = new TileEffectDataFile();
            DataFile file = new DataFile(CommonURIs.GetTileEffectScript(tileD));
            teff = reader.Read(file);

            teff.ID = tileD.ID;
            teff.Name = tileD.Name;
            teff.VfxID = tileD.AnimationURI ?? "";

            if (!string.IsNullOrEmpty(tileD.ImageURI))
                teff.Texture = ResourceManager.LoadTexture(CommonURIs.GetTileEffectImage(tileD));
            if (!string.IsNullOrEmpty(tileD.ModelURI))
                teff.Asset = ResourceManager.GetModel(CommonURIs.GetTileEffectModel(tileD), ModelPurpose.Game);

            TileEffectTemplates.Add(teff);
            return teff.Instance();
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
