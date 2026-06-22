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
            RB.BeginMode3D(Camera.Camera);
        }

        public void End()
        {
            RB.EndMode3D();
        }
    }
}
