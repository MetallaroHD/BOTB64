using System.Numerics;
using RB = Raylib_cs.Raylib;
using RL = Raylib_cs;

namespace BOTB64.Graphics.G3D
{
    public class Viewport
    {
        public CameraController Camera { get; }

        public Viewport()
        {
            Camera = new CameraController();
            Camera.CreateNewCamera();
        }

        public Vector3 GetMouseXZ()
        {
            return Camera.GetMouseXZ();
        }

        public void Update(float dt)
        {
            Camera.UpdateCamera(dt);
        }

        public void Begin()
        {
            // Raylib's default near plane (0.01) leaves too little depth-buffer precision
            // near the camera to resolve the sub-unit Y offsets Board.cs uses to layer tile
            // fill/effects/highlights, which showed up as PS1-style z-fighting flicker.
            // Standard perspective depth buffers concentrate precision near the near plane,
            // so tightening just this (0.01 -> 0.1) fixes that without needing to touch the
            // far plane - keep far at Raylib's own default (1000) so distant terrain/
            // environment geometry isn't clipped (an earlier pass here dropped it to 100,
            // which cut render distance well short of what the environment actually needs).
            RL.Rlgl.SetClipPlanes(0.1, 1000);
            RB.BeginMode3D(Camera.Camera);
        }

        public void End()
        {
            RB.EndMode3D();
        }
    }
}
