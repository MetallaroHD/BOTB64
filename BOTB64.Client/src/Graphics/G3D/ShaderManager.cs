using System.Numerics;

namespace BOTB64.Graphics.G3D
{ 
    public static class ShaderManager
    {
        public static WorldShader WS { get; private set; }
        public static CharSelectShader CS { get; private set; }

        public static void LoadWorld(string vs, string fs)
        {
            WS = new WorldShader(vs, fs);
        }
        public static void LoadCharSelect(string vs, string fs)
        {
            CS = new CharSelectShader(vs, fs);
        }

        public static void UpdateWorld()
        {
            WS.SetLighting(Vector3.Normalize(new Vector3(-0.35f, -1.0f, -0.35f)), new Vector3(1.4f, 1.4f, 1.4f));
            WS.SetFillLight(new Vector3(0, -1, 0), new Vector3(0.5f, 0.55f, 0.7f));
            WS.SetFog(new Vector4(0.5f, 0.5f, 0.55f, 1.0f), 110f, 175f);
        }
        public static void UpdateCharSelect(Vector3 camPos)
        {
            CS.SetCamera(camPos);
            CS.SetLightColor(new Vector3(1.3f, 1.3f, 1.3f));
        }
        public static void UpdateCameraPosition(Vector3 pos)
        {
            WS.SetCameraPosition(pos);
        }
        public static void Unload()
        {
            WS.Dispose();
            CS?.Dispose();
        }
    }
}
