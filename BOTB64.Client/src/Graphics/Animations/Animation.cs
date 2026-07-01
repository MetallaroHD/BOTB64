namespace BOTB64.Graphics.Animations
{
    public abstract class Animation
    {
        public bool IsComplete { get; protected set; }
        public bool IsBlocking { get; protected set; }

        public virtual void Start() { }
        public abstract void Update(float dt);
        public virtual void OnComplete() { }
    }
}
