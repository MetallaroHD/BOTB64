using BOTB64.Graphics.G3D;
using Raylib_cs;
using System.Numerics;
using RB = Raylib_cs.Raylib;
using RL = Raylib_cs;

namespace BOTB64.Graphics
{
    public static class Graphics
    {
        private static Shader Shader;
        private static bool ShaderReady;
        private static int LightDirLoc;
        private static int LightColorLoc;
        private static Vector3 LightDir;
        private static Vector3 LightColor;

        private static CameraController? Camera;

        public static void Initialize(int width, int height, string title)
        {
            RB.InitWindow(width, height, title);
            RB.SetTargetFPS(60);
            Camera = new CameraController();
            Camera.CreateNewCamera();
        }

        public static bool CompileShader(string vsPath, string fsPath)
        {
            Shader = new Shader(vsPath, fsPath);
            ShaderReady = Shader.IsValid();
            if (!ShaderReady) return false;

            LightDirLoc = RB.GetShaderLocation(Shader.Get(), "lightDir");
            LightColorLoc = RB.GetShaderLocation(Shader.Get(), "lightColor");
            LightDir = Vector3.Normalize(new Vector3(0, -1, 0));
            LightColor = new Vector3(1.5f, 1.5f, 1.5f);
            return true;
        }

        public static void Unload()
        {
            if (ShaderReady) 
                Shader.Unload();
            RB.CloseWindow();
        }

        public static void RenderFrame(Action? sceneDrawCallback = null)
        {
            Camera?.UpdateCamera(Engine.Engine.DeltaTime);
            if (ShaderReady) UploadShaderUniforms();

            RB.BeginDrawing();
            RB.ClearBackground(RL.Color.SkyBlue);

            if (Camera != null)
            {
                RB.BeginMode3D(Camera.Camera);
                if (ShaderReady) 
                    RB.BeginShaderMode(Shader.Get());

                sceneDrawCallback?.Invoke();    // state renders here

                if (ShaderReady) RB.EndShaderMode();
                RB.EndMode3D();
            }

            RB.DrawFPS(10, 10);
            RB.EndDrawing();
        }

        private static void Draw3D()
        {
            bool hasShader = Shader.TryGet(out var shader);
            if (hasShader) RB.BeginShaderMode(shader);

            RB.DrawSphere(Vector3.Zero, 1f, RL.Color.Red);

            if (hasShader) RB.EndShaderMode();
        }

        private static void DrawUI()
        {
            RB.DrawFPS(10, 10);
        }

        public static void DrawModel(RL.Model model, Vector3 position, float scale, RL.Color tint)
            => RB.DrawModel(model, position, scale, tint);

        public static void SetModelShader(RL.Model model)
        {
            if (!ShaderReady) return;
            unsafe
            {
                for (int i = 0; i < model.MaterialCount; i++)
                    model.Materials[i].Shader = Shader.Get();
            }
        }

        private static void UploadShaderUniforms()
        {
            RB.SetShaderValue(Shader.Get(), LightDirLoc, LightDir, RL.ShaderUniformDataType.Vec3);
            RB.SetShaderValue(Shader.Get(), LightColorLoc, LightColor, RL.ShaderUniformDataType.Vec3);
        }
    }
}