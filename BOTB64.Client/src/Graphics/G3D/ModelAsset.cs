using BOTB64.Runtime;
using RL = Raylib_cs;
using RB = Raylib_cs.Raylib;

namespace BOTB64.Graphics.G3D
{
    public enum ModelPurpose
    {
        None = 0,
        CharSelect = 1,
        Game = 2,
    }

    public class ModelAsset : IDisposable
    {
        public RL.Model Model { get; private set; }

        public ModelAsset(string relPath, ModelPurpose purpose)
        {
            Model = ResourceManager.LoadModel(relPath);
            RL.Shader? shader = null;

            switch (purpose)
            {
                case ModelPurpose.None:
                    break;
                case ModelPurpose.CharSelect:
                    shader = ShaderManager.CS.GetShader();
                    break;
                case ModelPurpose.Game:
                    shader = ShaderManager.WS.GetShader();
                    break;
            }

            unsafe
            {
                if (shader != null)
                {
                    for (int i = 0; i < Model.MaterialCount; i++)
                    {
                        Model.Materials[i].Shader = shader.Value;

                        int loc = RB.GetShaderLocation(Model.Materials[i].Shader, "texture0");
                        RB.SetShaderValueTexture(Model.Materials[i].Shader, loc,
                            Model.Materials[i].Maps[(int)RL.MaterialMapIndex.Albedo].Texture);
                    }
                }
            }
        }

        public void Dispose()
        {
            RB.UnloadModel(Model);
        }
    }
}
