using BOTB64.Entities;
using BOTB64.Graphics.Vfx;
using BOTB64.Runtime;

namespace BOTB64.Graphics.Animations
{
    public class AuraVfxAnimation : Animation
    {
        private readonly Character _character;
        private readonly Aura _aura;
        private LoopingVfx _vfx;

        public AuraVfxAnimation(Character character, Aura aura)
        {
            _character = character;
            _aura = aura;
            IsBlocking = false; // runs forever alongside everything else
        }

        public override void Start()
        {
            if (!string.IsNullOrEmpty(_aura.VfxID))
                _vfx = VfxManager.PlayLooping(_aura.VfxID,
                    () => _character.IsAnimating ? _character.VisualPosition : HexAlgo.HexToWorld(_character.Position));
        }

        public override void Update(float dt)
        {
            // The aura can be removed from a handful of scattered call sites; polling here
            // avoids having to hook all of them individually.
            if (!_character.CurrentAuras.Contains(_aura))
                Stop();
        }

        public void Stop()
        {
            _vfx?.Stop();
            IsComplete = true;
        }
    }
}
