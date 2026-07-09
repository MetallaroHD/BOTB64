using System.Numerics;
using RB = Raylib_cs.Raylib;
using RL = Raylib_cs;
namespace BOTB64.Graphics.G3D
{
    public class CharSelectShader : IDisposable
    {
        private readonly Shader Shader;
        private readonly int CamPosLoc;
        private readonly int LightColorLoc;
        public CharSelectShader(string vs, string fs)
        {
            Shader = new Shader(vs, fs);
            if (!Shader.IsValid())
                throw new Exception("Failed to load champ select shader");
            CamPosLoc = RB.GetShaderLocation(Shader.Handle, "camPos");
            LightColorLoc = RB.GetShaderLocation(Shader.Handle, "lightColor");
        }
        public RL.Shader GetShader() => Shader.Handle;
        public void SetCamera(Vector3 camPos)
        {
            RB.SetShaderValue(Shader.Handle, CamPosLoc, camPos, RL.ShaderUniformDataType.Vec3);
        }
        public void SetLightColor(Vector3 color)
        {
            RB.SetShaderValue(Shader.Handle, LightColorLoc, color, RL.ShaderUniformDataType.Vec3);
        }
        public void Dispose()
        {
            Shader.Dispose();
        }
    }
}