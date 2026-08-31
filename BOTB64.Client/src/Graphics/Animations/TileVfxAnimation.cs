using BOTB64.Entities;
using BOTB64.Graphics.Vfx;

namespace BOTB64.Graphics.Animations
{
    public class TileVfxAnimation : Animation
    {
        private readonly Tile _tile;
        private readonly TileEffect _effect;
        private LoopingVfx _vfx;

        public TileVfxAnimation(Tile tile, TileEffect effect)
        {
            _tile = tile;
            _effect = effect;
            IsBlocking = false; // runs forever alongside everything else
        }

        public override void Start()
        {
            if (!string.IsNullOrEmpty(_effect.VfxID))
                _vfx = VfxManager.PlayLooping(_effect.VfxID, () => _tile.WorldPosition);
        }

        public override void Update(float dt)
        {
            if (!_tile.Effects.Contains(_effect))
                Stop();
        }

        public void Stop()
        {
            _vfx?.Stop();
            IsComplete = true;
        }
    }
}
