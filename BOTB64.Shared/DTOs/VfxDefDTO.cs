namespace BOTB64.Shared.DTOs
{
    public class VfxDefDTO
    {
        public string ID { get; set; }
        public string Type { get; set; }
        public string Animation { get; set; }
        public string ImpactVfx { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public float TravelDuration { get; set; }
        public string TintHex { get; set; }
        // World-space Y bump applied to every position this VFX renders at, so a billboard
        // doesn't sit flat at ground level (Y=0) and get depth-occluded by whatever's standing
        // on that same tile - e.g. the target character's own leg/base geometry.
        public float HeightOffset { get; set; }
    }
}
