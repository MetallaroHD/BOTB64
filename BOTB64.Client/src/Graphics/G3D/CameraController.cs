using BOTB64.Runtime;
using Raylib_cs;
using System.Numerics;
using RL = Raylib_cs;
using RB = Raylib_cs.Raylib;

namespace BOTB64.Graphics.G3D
{
    public class CameraController
    {
        public Vector3 Position => Camera.Position;

        private const int MaxDistance = 25;
        private bool Enabled = true;

        private float Distance = 15.0f;
        private float Yaw = 45.0f;
        private float Pitch = 30.0f;

        private float PanSpeed = 10.0f;
        private float RotSpeed = 0.25f;
        
        private Vector3 Forward = new Vector3();
        private Vector3 Right = new Vector3();

        Vector3 DefaultCameraPos = new Vector3(0, 1f, 0);
        Vector3 DefaultCameraTarget = new Vector3(0, 1f, 0);

        Vector3 Offset = new Vector3();

        public RL.Camera3D Camera;

        public void Enable() => Enabled = true;
        public void Disable() => Enabled = false;
        public bool IsEnabled() => Enabled;

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

            if(Enabled)
            {
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

                ClampTargetToRadius();
            }

            Camera.Position = Camera.Target + Offset;
        }

        private void UpdateVectors()
        {
            Forward.X = MathF.Sin(Yaw * Transform3D.PIO180);
            Forward.Z = MathF.Cos(Yaw * Transform3D.PIO180);
            Forward = Vector3.Normalize(Forward);

            Right = Vector3.Normalize(Vector3.Cross(Forward, Vector3.UnitY));
        }

        private void UpdateOffset()
        {
            Offset.X = Distance * MathF.Cos(Pitch * Transform3D.PIO180) * MathF.Sin(Yaw * Transform3D.PIO180);
            Offset.Y = Distance * MathF.Sin(Pitch * Transform3D.PIO180);
            Offset.Z = Distance * MathF.Cos(Pitch * Transform3D.PIO180) * MathF.Cos(Yaw * Transform3D.PIO180);
        }

        public Vector3 GetMouseXZ()
        {
            Ray ray = RB.GetScreenToWorldRay(InputManager.MousePosition, Camera);

            if(MathF.Abs(ray.Direction.Y) > 0.0001f)
            {
                float t = -ray.Position.Y / ray.Direction.Y;
                return ray.Position + ray.Direction * t;
            }

            return Vector3.Zero;
        }

        private void ClampTargetToRadius()
        {
            Vector2 targetXZ = new Vector2(Camera.Target.X, Camera.Target.Z);

            if (targetXZ.Length() > MaxDistance)
            {
                targetXZ = Vector2.Normalize(targetXZ) * MaxDistance;

                Camera.Target.X = targetXZ.X;
                Camera.Target.Z = targetXZ.Y;
            }
        }
    }
}
