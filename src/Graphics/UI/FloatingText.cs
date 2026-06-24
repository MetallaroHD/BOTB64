using RL = Raylib_cs;
using System.Numerics;

namespace BOTB64.Graphics.UI
{
    public class FloatingText
    {
        public string Text;

        public Vector3 WorldPosition;

        public float Lifetime;
        public float MaxLifetime;

        public float RiseSpeed;
        public float Size;

        public RL.Color Color;
    }
}
