using System.Numerics;
using BOTB64.Shared.DTOs;
using RL = Raylib_cs;

namespace BOTB64.Graphics.Vfx
{
    public class ProjectileVfx : VfxInstance
    {
        private readonly Vector3 _from;
        private readonly Vector3 _to;
        private readonly float _duration;
        private float _elapsed;

        public ProjectileVfx(VfxDefDTO def, AnimationAsset asset, Vector3 from, Vector3 to)
        {
            Def = def;
            _from = from;
            _to = to;
            _duration = MathF.Max(0.01f, def.TravelDuration);
            Player = new SpriteAnimationPlayer(asset, loop: true);
        }

        public override void Update(float dt)
        {
            _elapsed += dt;
            Player.Update(dt);

            if (_elapsed >= _duration)
                IsComplete = true;
        }

        private Vector3 CurrentPosition()
        {
            return Vector3.Lerp(_from, _to, Math.Clamp(_elapsed / _duration, 0f, 1f));
        }

        public override void Draw(RL.Camera3D camera)
        {
            RL.Raylib.DrawBillboardRec(camera, Player.Asset.Texture, Player.CurrentFrameRect(),
                CurrentPosition(), new Vector2(Def.Width, Def.Height), TintOf(Def));
        }

        public override void OnComplete()
        {
            if (!string.IsNullOrEmpty(Def.ImpactVfx))
                VfxManager.PlayInstant(Def.ImpactVfx, _to);
        }
    }
}
