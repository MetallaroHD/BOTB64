using System.Numerics;
using BOTB64.Shared.DTOs;
using RL = Raylib_cs;

namespace BOTB64.Graphics.Vfx
{
    public class LoopingVfx : VfxInstance
    {
        private readonly Func<Vector3> _positionProvider;

        public LoopingVfx(VfxDefDTO def, AnimationAsset asset, Func<Vector3> positionProvider)
        {
            Def = def;
            _positionProvider = positionProvider;
            Player = new SpriteAnimationPlayer(asset, loop: true);
        }

        public override void Update(float dt)
        {
            Player.Update(dt);
        }

        public override void Draw(RL.Camera3D camera)
        {
            RL.Raylib.DrawBillboardRec(camera, Player.Asset.Texture, Player.CurrentFrameRect(),
                _positionProvider(), new Vector2(Def.Width, Def.Height), TintOf(Def));
        }
    }
}
