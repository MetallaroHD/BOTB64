using RB = Raylib_cs.Raylib;

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
