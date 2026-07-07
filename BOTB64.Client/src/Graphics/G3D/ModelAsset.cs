using BOTB64.Runtime;
using RL = Raylib_cs;
using RB = Raylib_cs.Raylib;

namespace BOTB64.Graphics.G3D
{
    public class ModelAsset : IDisposable
    {
        public RL.Model Model { get; private set; }

        public ModelAsset(string relPath)
        {
            Model = ResourceManager.LoadModel(relPath);

            for (int i = 0; i < Model.MaterialCount; i++)
            {
                unsafe
                {
                    Model.Materials[i].Shader = ShaderManager.WS.GetShader();

                    int loc = RB.GetShaderLocation(Model.Materials[i].Shader, "texture0");
                    RB.SetShaderValueTexture(Model.Materials[i].Shader, loc,
                        Model.Materials[i].Maps[(int)RL.MaterialMapIndex.Albedo].Texture);
                }
            }
        }

        public void Dispose()
        {
            RB.UnloadModel(Model);
        }
    }
}
