using System.Numerics;
using BOTB64.Shared.DTOs;
using RL = Raylib_cs;

namespace BOTB64.Graphics.Vfx
{
    public class InstantVfx : VfxInstance
    {
        private readonly Vector3 _position;

        public InstantVfx(VfxDefDTO def, AnimationAsset asset, Vector3 position)
        {
            Def = def;
            _position = position;
            Player = new SpriteAnimationPlayer(asset, loop: false);
        }

        public override void Update(float dt)
        {
            Player.Update(dt);
            if (Player.IsComplete)
                IsComplete = true;
        }

        public override void Draw(RL.Camera3D camera)
        {
            RL.Raylib.DrawBillboardRec(camera, Player.Asset.Texture, Player.CurrentFrameRect(), _position, new Vector2(Def.Width, Def.Height), TintOf(Def));
        }
    }
}
