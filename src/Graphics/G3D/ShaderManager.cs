using System.Numerics;

namespace BOTB64.Graphics.G3D
{ 
    public static class ShaderManager
    {
        public static WorldShader WS { get; private set; }

        public static void Load(string vs, string fs)
        {
            WS = new WorldShader(vs, fs);
        }

        public static void Update()
        {
            WS.SetLighting(Vector3.Normalize(new Vector3(-0.35f, -1.0f, -0.35f)), new Vector3(1.4f, 1.4f, 1.4f));
            WS.SetFillLight(new Vector3(0, -1, 0), new Vector3(0.5f, 0.55f, 0.7f));
        }

        public static void Unload()
        {
            WS.Dispose();
        }
    }
}
