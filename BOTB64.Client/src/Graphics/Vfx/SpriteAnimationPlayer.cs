using RL = Raylib_cs;

namespace BOTB64.Graphics.Vfx
{
    public class SpriteAnimationPlayer
    {
        public AnimationAsset Asset;
        public bool Loop;
        public bool IsComplete { get; private set; }

        private float _elapsed;

        public SpriteAnimationPlayer(AnimationAsset asset, bool loop)
        {
            Asset = asset;
            Loop = loop;
        }

        public void Update(float dt)
        {
            if (IsComplete)
                return;

            _elapsed += dt;

            if (_elapsed >= Asset.Duration)
            {
                if (Loop)
                    _elapsed %= Asset.Duration;
                else
                {
                    _elapsed = Asset.Duration;
                    IsComplete = true;
                }
            }
        }

        public RL.Rectangle CurrentFrameRect()
        {
            int frame = Asset.Def.FrameCount <= 1
                ? 0
                : Math.Min((int)(_elapsed * Asset.Def.FPS), Asset.Def.FrameCount - 1);

            return Asset.GetFrameRect(frame);
        }
    }
}
