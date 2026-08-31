using BOTB64.Shared.DTOs;
using RL = Raylib_cs;

namespace BOTB64.Graphics.Vfx
{
    public class AnimationAsset
    {
        public AnimationDefDTO Def;
        public RL.Texture2D Texture;

        public float Duration => Def.FrameCount / Def.FPS;

        public AnimationAsset(AnimationDefDTO def, RL.Texture2D texture)
        {
            Def = def;
            Texture = texture;
        }

        public RL.Rectangle GetFrameRect(int frameIndex)
        {
            return new RL.Rectangle(frameIndex * Def.FrameWidth, 0, Def.FrameWidth, Def.FrameHeight);
        }
    }
}
