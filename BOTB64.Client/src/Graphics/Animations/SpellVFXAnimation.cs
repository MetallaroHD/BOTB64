using BOTB64.Entities;
using BOTB64.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOTB64.Graphics.Animations
{
    public class SpellVfxAnimation : Animation
    {
        private readonly Character _character;
        //private readonly VfxInstance _vfx;

        public SpellVfxAnimation(Character character/*, VfxInstance vfx*/)
        {
            _character = character;
            //_vfx = vfx;
            IsBlocking = false; // runs forever alongside everything else
        }

        public override void Update(float dt)
        {
            // Animation just runs and that's it
        }

        public void Stop() => IsComplete = true;
    }
}
