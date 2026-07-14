using BOTB64.Entities;
using BOTB64.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOTB64.Graphics.Animations
{
    public class TileVfxAnimation : Animation
    {
        private readonly Tile _tile;
        //private readonly VfxInstance _vfx;

        public TileVfxAnimation(Tile tile/*, VfxInstance vfx*/)
        {
            _tile = tile;
            //_vfx = vfx;
            IsBlocking = false; // runs forever alongside everything else
        }

        public override void Update(float dt)
        {
            // Aura tracks character position continuously
            //_vfx.Position = HexAlgo.HexToWorld(_character.Position);
            //_vfx.Update(dt);
            // IsComplete stays false until the aura is removed
        }

        public void Stop() => IsComplete = true;
    }
}
