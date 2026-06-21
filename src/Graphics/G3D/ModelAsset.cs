using BOTB64.Runtime;
using Raylib_cs;
using RL = Raylib_cs;
using RB = Raylib_cs.Raylib;

namespace BOTB64.Graphics.G3D
{
    public class ModelAsset : IDisposable
    {
        public RL.Model Model { get; private set; }

        public ModelAsset(DataFile df)
        {
            if (!df.Exists())
                throw new Exception("Invalid model path!");

            Model = RB.LoadModel(df.Path);

            for (int i=0; i<Model.MaterialCount; i++)
            {
                unsafe
                {
                    Model.Materials[i].Shader = ShaderManager.WS.GetShader();
                }
            }
        }

        public void Dispose()
        {
            RB.UnloadModel(Model);
        }
    }
}
