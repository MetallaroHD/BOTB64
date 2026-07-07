using RL = Raylib_cs;
using RB = Raylib_cs.Raylib;
using System.Numerics;
using BOTB64.Graphics.G3D;

namespace BOTB64.Graphics.UI
{
    public static class FloatingTextManager
    {
        private static readonly List<FloatingText> Texts = new();

        public static void Add(string text, Vector3 worldPosition, float lifetime = 1.5f, float riseSpeed = 1.5f, float size = 25f, RL.Color? color = null)
        {
            Texts.Add(new FloatingText
            {
                Text = text,
                WorldPosition = worldPosition,
                Lifetime = lifetime,
                MaxLifetime = lifetime,
                RiseSpeed = riseSpeed,
                Size = size,
                Color = color ?? RL.Color.Red
            });
        }

        public static void Update(float dt)
        {
            for (int i = Texts.Count - 1; i >= 0; i--)
            {
                var t = Texts[i];

                t.Lifetime -= dt;

                t.WorldPosition += Vector3.UnitY * t.RiseSpeed * dt;

                if (t.Lifetime <= 0)
                    Texts.RemoveAt(i);
            }
        }

        public static void Draw(Viewport vp)
        {
            foreach (var t in Texts)
            {
                Vector2 screenPos = RB.GetWorldToScreen(t.WorldPosition, vp.Camera.Camera);

                if (!IsInFrontOfCamera(t.WorldPosition, vp.Camera.Camera))
                    continue;

                float alpha = Math.Clamp(t.Lifetime / t.MaxLifetime, 0f, 1f);

                RL.Color c = t.Color;
                c.A = (byte)(255 * alpha);

                string text = t.Text;
                int textWidth = RB.MeasureText(text, (int)t.Size);

                RB.DrawText(text, (int)(screenPos.X - textWidth / 2), (int)screenPos.Y, (int)t.Size, c);
            }
        }

        private static bool IsInFrontOfCamera(Vector3 worldPos, RL.Camera3D camera)
        {
            // Camera forward direction
            Vector3 forward =
                Vector3.Normalize(camera.Target - camera.Position);

            Vector3 toPoint =
                worldPos - camera.Position;

            return Vector3.Dot(forward, toPoint) > 0;
        }

        public static void Clear()
        {
            Texts.Clear();
        }
    }
}
