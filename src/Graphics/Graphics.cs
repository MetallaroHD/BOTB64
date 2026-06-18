using BOTB64.Core;
using System.Numerics;
using RB = Raylib_cs.Raylib;
using RL = Raylib_cs;

namespace BOTB64.Graphics
{
    public static class Graphics
    {
        // Should be wrapped in a shader class if we need more than 1
        static RL.Shader? GlobalShader;
        static CameraController? Camera;

        static int LightDirLoc = 0;
        static int LightColorLoc = 0;

        static Vector3 LightDir = new();
        static Vector3 LightColor = new();

        public static void Initialize()
        {
            RB.InitWindow(1280, 720, "BOTB64");
            RB.SetTargetFPS(60);
            Init3D(); //for now its here bc we have no menu
        }

        public static bool CompileShader(string vertexPath, string fragmentPath)
        {
            DataFile vertex = new DataFile(vertexPath);
            DataFile fragment = new DataFile(fragmentPath);
            GlobalShader = RB.LoadShader(vertex.Path, fragment.Path);

            if(GlobalShader == null)
                return false;

            LightDirLoc = RB.GetShaderLocation(GlobalShader.GetValueOrDefault(), "lightDir");
            LightColorLoc = RB.GetShaderLocation(GlobalShader.GetValueOrDefault(), "lightColor");

            LightDir = Vector3.Normalize(new Vector3(0, -1, 0));
            LightColor = new Vector3(1.5f, 1.5f, 1.5f);

            return GlobalShader != null;
        }

        public static void SetModelShader(RL.Model model)
        {
            if (GlobalShader == null)
                return;

            unsafe
            {
                for (int i = 0; i < model.MaterialCount; i++)
                {
                    model.Materials[i].Shader = GlobalShader.GetValueOrDefault();
                }
            }
        }

        public static void Unload()
        {
            if (GlobalShader != null)
                RB.UnloadShader(GlobalShader.GetValueOrDefault());
            RB.CloseWindow();
        }

        public static void Init3D()
        {
            Camera = new CameraController();
            Camera.CreateNewCamera();
        }

        public static void Update(float dt)
        {
            if(Camera != null)
                Camera.UpdateCamera(dt);
            UpdateShaders();
            Draw();
        }

        private static void Draw()
        {
            //if in game ensure 3d env
            RB.BeginDrawing();
            RB.ClearBackground(RL.Color.SkyBlue);
            RB.BeginMode3D(Camera.Camera);
            RB.BeginShaderMode(GlobalShader.GetValueOrDefault());
            //draw all
            RB.DrawSphere(new Vector3(0, 0, 0), 1, RL.Color.Red); //for debugging
            RB.EndShaderMode();
            RB.EndMode3D();
            RB.EndDrawing();
        }

        private static void UpdateShaders() 
        {
            if (GlobalShader == null)
                return;
            RB.SetShaderValue(GlobalShader.GetValueOrDefault(), LightDirLoc, LightDir, RL.ShaderUniformDataType.Vec3);
            RB.SetShaderValue(GlobalShader.GetValueOrDefault(), LightColorLoc, LightColor, RL.ShaderUniformDataType.Vec3);
        }
    } 
}
