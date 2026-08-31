using System.Numerics;
using BOTB64.Shared.DTOs;
using RL = Raylib_cs;

namespace BOTB64.Graphics.Vfx
{
    public class BeamVfx : VfxInstance
    {
        private readonly Vector3 _from;
        private readonly Vector3 _to;

        public BeamVfx(VfxDefDTO def, AnimationAsset asset, Vector3 from, Vector3 to)
        {
            Def = def;
            _from = from;
            _to = to;
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
            DrawQuad(camera, Player.Asset.Texture, Player.CurrentFrameRect(), _from, _to, Def.Height, TintOf(Def));
        }

        // Stretches a texture as a flat quad along the world-space A->B line, thin edge billboarding
        // toward the camera. DrawBillboardPro can't do this: raylib derives one quad axis from the
        // camera's right vector internally regardless of the "up"/rotation parameters passed in, so
        // there's no way to lock an axis to an arbitrary world direction through that API.
        public static void DrawQuad(RL.Camera3D camera, RL.Texture2D texture, RL.Rectangle source,
            Vector3 a, Vector3 b, float thickness, RL.Color tint)
        {
            Vector3 d = b - a;
            if (d.LengthSquared() < 0.0001f)
                return;
            d = Vector3.Normalize(d);

            Vector3 camForward = Vector3.Normalize(camera.Target - camera.Position);
            Vector3 t = Vector3.Cross(d, camForward);
            if (t.LengthSquared() < 0.0001f)
                t = Vector3.Cross(d, camera.Up);
            if (t.LengthSquared() < 0.0001f)
                return;

            t = Vector3.Normalize(t) * (thickness / 2f);

            Vector3 topA = a + t, botA = a - t, topB = b + t, botB = b - t;

            float u0 = source.X / texture.Width;
            float v0 = source.Y / texture.Height;
            float u1 = (source.X + source.Width) / texture.Width;
            float v1 = (source.Y + source.Height) / texture.Height;

            RL.Rlgl.SetTexture(texture.Id);
            RL.Rlgl.Begin((int)RL.DrawMode.Quads);
            RL.Rlgl.Color4ub(tint.R, tint.G, tint.B, tint.A);

            RL.Rlgl.TexCoord2f(u0, v1);
            RL.Rlgl.Vertex3f(botA.X, botA.Y, botA.Z);

            RL.Rlgl.TexCoord2f(u1, v1);
            RL.Rlgl.Vertex3f(botB.X, botB.Y, botB.Z);

            RL.Rlgl.TexCoord2f(u1, v0);
            RL.Rlgl.Vertex3f(topB.X, topB.Y, topB.Z);

            RL.Rlgl.TexCoord2f(u0, v0);
            RL.Rlgl.Vertex3f(topA.X, topA.Y, topA.Z);

            RL.Rlgl.End();
            RL.Rlgl.SetTexture(0);
        }
    }
}
