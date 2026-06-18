using BOTB64.Runtime;
using RL = Raylib_cs;
using RB = Raylib_cs.Raylib;

namespace BOTB64.Graphics.G3D
{
    internal class Shader
    {
        RL.Shader? RLShader = null;

        public Shader(string vertexFile, string fragmentFile)
        {
            DataFile vf = new DataFile(vertexFile);
            DataFile ff = new DataFile(fragmentFile);

            if (!vf.Exists() || !ff.Exists())
                return;

            RLShader = RB.LoadShader(vf.Path, ff.Path);
            if (!RLShader.HasValue)
                RLShader = null;
        }

        public bool IsValid()
        {
            return RLShader.HasValue;
        }

        public RL.Shader Get()
        { 
            return RLShader.Value;
        }

        public void Unload()
        {
            if(IsValid())
                RB.UnloadShader(RLShader.Value);
        }
    }
}
