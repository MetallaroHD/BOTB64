using RL = Raylib_cs;
using RB = Raylib_cs.Raylib;

namespace BOTB64.Graphics.G3D
{
    public class ModelInstance
    {
        public ModelAsset Asset { get; }
        public Transform Transform { get; } = new();

        public RL.Color Tint = RL.Color.White;

        public ModelInstance(ModelAsset asset)
        {
            Asset = asset;
        }

        public void Draw()
        {
            RB.DrawModelEx(Asset.Model, Transform.Position, Transform.RotationAxis, Transform.RotationAngle, Transform.Scale, Tint);
        }
    }
}
