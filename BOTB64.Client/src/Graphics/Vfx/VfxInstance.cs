using BOTB64.Shared.DTOs;
using RL = Raylib_cs;

namespace BOTB64.Graphics.Vfx
{
    public enum VfxType
    {
        Projectile,
        Beam,
        Instant,
        Looping,
        Tether
    }

    public abstract class VfxInstance
    {
        public VfxDefDTO Def;
        public SpriteAnimationPlayer Player;
        public bool IsComplete { get; protected set; }

        public virtual void Start() { }
        public abstract void Update(float dt);
        public abstract void Draw(RL.Camera3D camera);
        public virtual void OnComplete() { }
        public virtual void Stop() => IsComplete = true;

        protected static RL.Color TintOf(VfxDefDTO def)
        {
            if (string.IsNullOrEmpty(def.TintHex) || def.TintHex.Length < 8)
                return RL.Color.White;

            byte r = Convert.ToByte(def.TintHex.Substring(0, 2), 16);
            byte g = Convert.ToByte(def.TintHex.Substring(2, 2), 16);
            byte b = Convert.ToByte(def.TintHex.Substring(4, 2), 16);
            byte a = Convert.ToByte(def.TintHex.Substring(6, 2), 16);
            return new RL.Color(r, g, b, a);
        }
    }
}
