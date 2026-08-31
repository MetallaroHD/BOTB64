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

            RefreshTextureFilter();
        }

        // Point filtering keeps the intended pixel-art look at the native 1280x720
        // canvas; above that (e.g. 1920x1080 fullscreen, Settings.Scale 1.5), the same
        // low-res textures get magnified across more physical pixels and point filtering's
        // hard texel edges become visibly blocky ("crispy") - bilinear softens just that
        // magnification without touching how things look at native resolution.
        public void RefreshTextureFilter()
        {
            RL.TextureFilter filter = Settings.Scale > 1.0f ? RL.TextureFilter.Bilinear : RL.TextureFilter.Point;

            unsafe
            {
                for (int i = 0; i < Model.MaterialCount; i++)
                    RB.SetTextureFilter(Model.Materials[i].Maps[(int)RL.MaterialMapIndex.Albedo].Texture, filter);
            }
        }

        public void Dispose()
        {
            RB.UnloadModel(Model);
        }
    }
}
