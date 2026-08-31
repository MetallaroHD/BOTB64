using System.Numerics;
using BOTB64.Engine;
using RL = Raylib_cs;

namespace BOTB64.Graphics.Vfx
{
    public static class VfxManager
    {
        private static readonly List<VfxInstance> Active = new();

        public static ProjectileVfx PlayProjectile(string vfxId, Vector3 from, Vector3 to)
        {
            var (def, asset) = Resolve(vfxId);
            if (def == null) return null;

            Vector3 up = Vector3.UnitY * def.HeightOffset;
            var vfx = new ProjectileVfx(def, asset, from + up, to + up);
            StartInstance(vfx);
            return vfx;
        }

        public static BeamVfx PlayBeam(string vfxId, Vector3 from, Vector3 to)
        {
            var (def, asset) = Resolve(vfxId);
            if (def == null) return null;

            Vector3 up = Vector3.UnitY * def.HeightOffset;
            var vfx = new BeamVfx(def, asset, from + up, to + up);
            StartInstance(vfx);
            return vfx;
        }

        public static InstantVfx PlayInstant(string vfxId, Vector3 position)
        {
            var (def, asset) = Resolve(vfxId);
            if (def == null) return null;

            var vfx = new InstantVfx(def, asset, position + Vector3.UnitY * def.HeightOffset);
            StartInstance(vfx);
            return vfx;
        }

        public static LoopingVfx PlayLooping(string vfxId, Func<Vector3> positionProvider)
        {
            var (def, asset) = Resolve(vfxId);
            if (def == null) return null;

            Vector3 up = Vector3.UnitY * def.HeightOffset;
            var vfx = new LoopingVfx(def, asset, () => positionProvider() + up);
            StartInstance(vfx);
            return vfx;
        }

        public static TetherVfx PlayTether(string vfxId, Func<Vector3> from, Func<Vector3> to)
        {
            var (def, asset) = Resolve(vfxId);
            if (def == null) return null;

            Vector3 up = Vector3.UnitY * def.HeightOffset;
            var vfx = new TetherVfx(def, asset, () => from() + up, () => to() + up);
            StartInstance(vfx);
            return vfx;
        }

        public static void Stop(VfxInstance instance) => instance?.Stop();

        public static void Update(float dt)
        {
            foreach (var v in Active)
                v.Update(dt);

            var completed = Active.Where(v => v.IsComplete).ToList();
            foreach (var v in completed)
            {
                v.OnComplete();
                Active.Remove(v);
            }
        }

        public static void Draw(RL.Camera3D camera)
        {
            foreach (var v in Active)
                v.Draw(camera);
        }

        public static void Clear() => Active.Clear();

        private static void StartInstance(VfxInstance vfx)
        {
            vfx.Start();
            Active.Add(vfx);
        }

        private static (Shared.DTOs.VfxDefDTO def, AnimationAsset asset) Resolve(string vfxId)
        {
            var def = VfxDatabase.GetVfx(vfxId);
            if (def == null)
            {
                Logger.Log($"VfxManager: unknown vfx '{vfxId}'");
                return (null, null);
            }

            var asset = VfxDatabase.GetAsset(def.Animation);
            if (asset == null)
            {
                Logger.Log($"VfxManager: unknown animation '{def.Animation}' for vfx '{vfxId}'");
                return (null, null);
            }

            return (def, asset);
        }
    }
}
