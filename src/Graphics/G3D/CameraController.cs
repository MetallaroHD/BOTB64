using BOTB64.Runtime;
using System.Numerics;
using RL = Raylib_cs;

namespace BOTB64.Graphics.G3D
{
    public class CameraController
    {
        private float Distance = 10.0f;
        private float Yaw = 45.0f;
        private float Pitch = 30.0f;

        private float PanSpeed = 10.0f;
        private float RotSpeed = 0.25f;

        private Vector3 Forward = new Vector3();
        private Vector3 Right = new Vector3();

        Vector3 DefaultCameraPos = new Vector3(0, 0.5f, 0);
        Vector3 DefaultCameraTarget = new Vector3(12.1243f, 7.0f, 0);

        Vector3 Offset = new Vector3();

        public RL.Camera3D Camera;

        public void CreateNewCamera()
        {
            Camera = new RL.Camera3D();
            Camera.Up = Vector3.UnitY;
            Camera.FovY = 45.0f;
            Camera.Projection = RL.CameraProjection.Perspective;

            Camera.Position = DefaultCameraPos;
            Camera.Target = DefaultCameraTarget;

            UpdateVectors();
            UpdateOffset();
        }

        public void UpdateCamera(float dt)
        {
            float panDistance = PanSpeed * dt;

            if (InputManager.IsMouseButtonDown(RL.MouseButton.Right))
            {
                Yaw -= InputManager.MouseDelta.X * RotSpeed;
                Pitch = Math.Clamp(Pitch + InputManager.MouseDelta.Y * RotSpeed, 0, 85.0f);

                UpdateVectors();
                UpdateOffset();
            }

            if (InputManager.ScrollDelta != 0)
            {
                Distance -= InputManager.ScrollDelta;
                Distance = Math.Clamp(Distance, 0.5f, 30.0f);

                UpdateOffset();
            }

            if (InputManager.IsKeyDown(RL.KeyboardKey.W))
                Camera.Target -= Forward * panDistance;
            if (InputManager.IsKeyDown(RL.KeyboardKey.S))
                Camera.Target += Forward * panDistance;
            if (InputManager.IsKeyDown(RL.KeyboardKey.A))
                Camera.Target += Right * panDistance;
            if (InputManager.IsKeyDown(RL.KeyboardKey.D))
                Camera.Target -= Right * panDistance;

            Camera.Position = Camera.Target + Offset;
        }

        private void UpdateVectors()
        {
            Forward.X = MathF.Sin(Yaw * Transform.PIO180);
            Forward.Z = MathF.Cos(Yaw * Transform.PIO180);
            Forward = Vector3.Normalize(Forward);

            Right = Vector3.Normalize(Vector3.Cross(Forward, Vector3.UnitY));
        }

        private void UpdateOffset()
        {
            Offset.X = Distance * MathF.Cos(Pitch * Transform.PIO180) * MathF.Sin(Yaw * Transform.PIO180);
            Offset.Y = Distance * MathF.Sin(Pitch * Transform.PIO180);
            Offset.Z = Distance * MathF.Cos(Pitch * Transform.PIO180) * MathF.Cos(Yaw * Transform.PIO180);
        }
    }
}
