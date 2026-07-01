using System.Numerics;

namespace BOTB64.Runtime
{
    public class Transform3D
    {
        public static float PIO180 = MathF.PI / 180;

        public Vector3 Position = Vector3.Zero;

        public Vector3 RotationAxis = Vector3.UnitY;
        public float RotationAngle = 0f;

        public Vector3 Scale = Vector3.One;
    }
}
