using System.Numerics;
using BOTB64.Shared.DTOs;
using RL = Raylib_cs;

namespace BOTB64.Graphics.Vfx
{
    public class TetherVfx : VfxInstance
    {
        private readonly Func<Vector3> _from;
        private readonly Func<Vector3> _to;

        public TetherVfx(VfxDefDTO def, AnimationAsset asset, Func<Vector3> from, Func<Vector3> to)
        {
            Def = def;
            _from = from;
            _to = to;
            Player = new SpriteAnimationPlayer(asset, loop: true);
        }

        public override void Update(float dt)
        {
            Player.Update(dt);
        }

        public override void Draw(RL.Camera3D camera)
        {
            BeamVfx.DrawQuad(camera, Player.Asset.Texture, Player.CurrentFrameRect(),
                _from(), _to(), Def.Height, TintOf(Def));
        }
    }
}
