using BOTB64.Runtime;
using BOTB64.Shared.DTOs;
using BOTB64.Shared.Files;

namespace BOTB64.Graphics.Vfx
{
    public static class VfxDatabase
    {
        public static readonly string AnimationsJSON = CommonURIs.GraphicsAnimDir + "animations.json";
        public static readonly string VfxJSON = CommonURIs.GraphicsAnimDir + "vfx.json";

        public static List<AnimationDefDTO> Animations = new();
        public static List<VfxDefDTO> Vfx = new();

        private static readonly Dictionary<string, AnimationAsset> AssetCache = new();

        public static void Init()
        {
            Animations = new JsonDataFile<AnimationDefDTO>().DeserializeAll(new DataFile(AnimationsJSON));
            Vfx = new JsonDataFile<VfxDefDTO>().DeserializeAll(new DataFile(VfxJSON));
            AssetCache.Clear();
        }

        public static VfxDefDTO GetVfx(string id)
        {
            return Vfx.FirstOrDefault(v => v.ID == id);
        }

        public static AnimationAsset GetAsset(string animationId)
        {
            if (AssetCache.TryGetValue(animationId, out var cached))
                return cached;

            var def = Animations.FirstOrDefault(a => a.ID == animationId);
            if (def == null)
                return null;

            var asset = new AnimationAsset(def, ResourceManager.LoadTexture(CommonURIs.GraphicsAnimDir + def.Texture));
            AssetCache[animationId] = asset;
            return asset;
        }
    }
}
