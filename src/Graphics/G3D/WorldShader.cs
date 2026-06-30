using System.Numerics;
using RB = Raylib_cs.Raylib;
using RL = Raylib_cs;

namespace BOTB64.Graphics.G3D
{
    public class WorldShader : IDisposable
    {
        private readonly Shader Shader;

        private readonly int LightDirLoc;
        private readonly int LightColorLoc;
        private readonly int FillDirLoc;
        private readonly int FillColorLoc;

        public WorldShader(string vs, string fs)
        {
            Shader = new Shader(vs, fs);

            if (!Shader.IsValid())
                throw new Exception("Failed to load world shader");

            LightDirLoc = RB.GetShaderLocation(Shader.Handle, "lightDir");
            LightColorLoc = RB.GetShaderLocation(Shader.Handle, "lightColor");
            FillDirLoc = RB.GetShaderLocation(Shader.Handle, "fillDir");
            FillColorLoc = RB.GetShaderLocation(Shader.Handle, "fillColor");
        }

        public RL.Shader GetShader()
        {
            return Shader.Handle;
        }

        public void SetLighting(Vector3 dir, Vector3 color)
        {
            RB.SetShaderValue(Shader.Handle, LightDirLoc, dir, RL.ShaderUniformDataType.Vec3);
            RB.SetShaderValue(Shader.Handle, LightColorLoc, color, RL.ShaderUniformDataType.Vec3);
        }

        public void SetFillLight(Vector3 dir, Vector3 color)
        {
            RB.SetShaderValue(Shader.Handle, FillDirLoc, dir, RL.ShaderUniformDataType.Vec3);
            RB.SetShaderValue(Shader.Handle, FillColorLoc, color, RL.ShaderUniformDataType.Vec3);
        }

        public void Dispose()
        {
            Shader.Dispose();
        }
    }
}
