using System.Numerics;

namespace BOTB64.Graphics.G3D
{ 
    public static class ShaderManager
    {
        public static WorldShader WS { get; private set; }

        public static void Load()
        {
            WS = new WorldShader();
        }

        public static void Update()
        {
            WS.SetLighting(Vector3.Normalize(new Vector3(0, -1, 0)), new Vector3(1.5f));
        }

        public static void Unload()
        {
            WS.Dispose();
        }
    }
}
