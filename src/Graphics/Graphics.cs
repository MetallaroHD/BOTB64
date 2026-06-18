using BOTB64.Runtime;
using BOTB64.Graphics.G3D;
using System.Numerics;
using RB = Raylib_cs.Raylib;
using RL = Raylib_cs;

namespace BOTB64.Graphics
{
    public static class Graphics
    {
        static Shader Shader;
        static CameraController? Camera;

        static int LightDirLoc = 0;
        static int LightColorLoc = 0;

        static Vector3 LightDir = new();
        static Vector3 LightColor = new();

        public static void Initialize()
        {
            RB.InitWindow(1280, 720, "BOTB64");
            RB.SetTargetFPS(60);
            Init3D();
        }

        public static bool CompileShader(string vertexPath, string fragmentPath)
        {
            Shader = new Shader(vertexPath, fragmentPath);

            if(!Shader.IsValid())
                return false;

            LightDirLoc = RB.GetShaderLocation(Shader.Get(), "lightDir");
            LightColorLoc = RB.GetShaderLocation(Shader.Get(), "lightColor");

            LightDir = Vector3.Normalize(new Vector3(0, -1, 0));
            LightColor = new Vector3(1.5f, 1.5f, 1.5f);

            return true;
        }

        public static void SetModelShader(RL.Model model)
        {
            if (!Shader.IsValid())
                return;

            unsafe
            {
                for (int i = 0; i < model.MaterialCount; i++)
                {
                    model.Materials[i].Shader = Shader.Get();
                }
            }
        }

        public static void Unload()
        {
            Shader.Unload();
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
            RB.BeginShaderMode(Shader.Get());
            //draw all
            RB.DrawSphere(new Vector3(0, 0, 0), 1, RL.Color.Red); //for debugging
            RB.EndShaderMode();
            RB.EndMode3D();
            RB.EndDrawing();
        }

        private static void UpdateShaders() 
        {
            if (!Shader.IsValid())
                return;
            RB.SetShaderValue(Shader.Get(), LightDirLoc, LightDir, RL.ShaderUniformDataType.Vec3);
            RB.SetShaderValue(Shader.Get(), LightColorLoc, LightColor, RL.ShaderUniformDataType.Vec3);
        }
    } 
}
