namespace BOTB64.Shared.DTOs
{
    public class AnimationDefDTO
    {
        public string ID { get; set; }
        public string Texture { get; set; }
        public int FrameWidth { get; set; }
        public int FrameHeight { get; set; }
        public int FrameCount { get; set; }
        public float FPS { get; set; }
        public bool Loop { get; set; }
    }
}
