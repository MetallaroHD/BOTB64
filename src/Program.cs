using BOTB64.Core;
using BOTB64.Entities;
using System.Diagnostics;
using System.Numerics;
using System.Reflection.Emit;
using RB = Raylib_cs.Raylib;
using RL = Raylib_cs;

namespace BOTB64
{
    public class Program
    {
        public static void Main()
        {
            RB.InitWindow(1280, 720, "BOTB64");
            RB.SetTargetFPS(60);

            Stopwatch clock = new Stopwatch();
            clock.Start();

            DataFile boardFile = new DataFile("Models\\Board\\Board.gltf");
            DataFile vertex = new DataFile("Shaders\\shader.vs");
            DataFile fragment = new DataFile("Shaders\\shader.fs");

            RL.Model board = RB.LoadModel(boardFile.ToString());
            RL.Shader shader = RB.LoadShader(vertex.ToString(), fragment.ToString());

            Board gameBoard = new Board();
            DataFile boardMap = new DataFile("Level\\board.botbmap");
            gameBoard.Generate(boardMap);

            unsafe
            {
                for (int i = 0; i < board.MaterialCount; i++)
                {
                    board.Materials[i].Shader = shader;
                }
            }

            RL.Camera3D camera = new RL.Camera3D();
            camera.Up = new Vector3(0, 1, 0);
            camera.FovY = 45.0f;
            camera.Projection = RL.CameraProjection.Perspective;

            Vector3 cameraPos = new Vector3(0, 1, 0);
            Vector3 cameraTarget = new Vector3(12.1243f, 7.0f, 0);
            float cameraDistance = 10.0f;
            float yaw = 45.0f;
            float pitch = 30.0f;

            int lightDirLoc = RB.GetShaderLocation(shader, "lightDir");
            int lightColorLoc = RB.GetShaderLocation(shader, "lightColor");

            Vector3 lightDir = Vector3.Normalize(new Vector3(0, -1, 0));
            Vector3 lightColor = new Vector3(1.5f, 1.5f, 1.5f);

            RL.BoundingBox bb = RB.GetModelBoundingBox(board);
            Vector3 center = new Vector3((bb.Max.X + bb.Min.X) / 2, (bb.Max.Y + bb.Min.Y) / 2 + 5, (bb.Max.Z + bb.Min.Z) / 2);

            Random rng = new Random();

            while (!RB.WindowShouldClose())
            {
                float dt = (float)clock.Elapsed.TotalSeconds;
                clock.Restart();

                float panSpeed = 10.0f * dt;

                Vector3 forward = Vector3.Normalize(
                    new Vector3(
                        MathF.Sin(yaw * MathF.PI / 180f),
                        0,
                        MathF.Cos(yaw * MathF.PI / 180f)
                    )
                );

                Vector3 right = Vector3.Normalize(
                    Vector3.Cross(forward, Vector3.UnitY)
                );

                if (RB.IsKeyDown(RL.KeyboardKey.W))
                    cameraTarget -= forward * panSpeed;

                if (RB.IsKeyDown(RL.KeyboardKey.S))
                    cameraTarget += forward * panSpeed;

                if (RB.IsKeyDown(RL.KeyboardKey.A))
                    cameraTarget += right * panSpeed;

                if (RB.IsKeyDown(RL.KeyboardKey.D))
                    cameraTarget -= right * panSpeed;

                cameraDistance -= RB.GetMouseWheelMove();

                cameraDistance = Math.Clamp(
                    cameraDistance,
                    2.0f,
                    50.0f
                );

                if (RB.IsMouseButtonDown(RL.MouseButton.Right))
                {
                    Vector2 delta = RB.GetMouseDelta();

                    yaw -= delta.X * 0.25f;
                    pitch += delta.Y * 0.25f;

                    pitch = Math.Clamp(
                        pitch,
                        0.0f,
                        85.0f
                    );
                }

                RL.Ray ray = RB.GetScreenToWorldRay(
                    RB.GetMousePosition(),
                    camera
                );

                Vector3 mouseWorld = Vector3.Zero;

                if (MathF.Abs(ray.Direction.Y) > 0.0001f)
                {
                    float t = -ray.Position.Y / ray.Direction.Y;

                    mouseWorld = ray.Position + ray.Direction * t;
                }

                //Engine.Update(InputManager.CommandQueue, dt);
                float yawRad = yaw * MathF.PI / 180f;
                float pitchRad = pitch * MathF.PI / 180f;

                Vector3 offset = new Vector3(
                    cameraDistance * MathF.Cos(pitchRad) * MathF.Sin(yawRad),
                    cameraDistance * MathF.Sin(pitchRad),
                    cameraDistance * MathF.Cos(pitchRad) * MathF.Cos(yawRad)
                );

                camera.Position = cameraTarget + offset;
                camera.Target = cameraTarget;
                camera.Up = Vector3.UnitY;

                //Graphics.Draw(Engine.GameState);
                RB.SetShaderValue(shader, lightDirLoc, lightDir, RL.ShaderUniformDataType.Vec3);
                RB.SetShaderValue(shader, lightColorLoc, lightColor, RL.ShaderUniformDataType.Vec3);

                RB.BeginDrawing();
                RB.ClearBackground(RL.Color.SkyBlue);
                RB.BeginMode3D(camera);
                RB.BeginShaderMode(shader);
                RB.DrawModelEx(board, new Vector3(0, 0, 0), new Vector3(0, 1, 0), 0, new Vector3(1, 1, 1), RL.Color.White);
                gameBoard.Draw();
                RB.DrawSphere(center, 1, RL.Color.Red);
                RB.EndShaderMode();
                RB.EndMode3D();

                RB.DrawText($"World: {mouseWorld.X:F2}, {mouseWorld.Z:F2}", 10, 40, 20, RL.Color.Black);
                RB.DrawFPS(10, 10);
                RB.EndDrawing();
            }

            RB.UnloadShader(shader);
            RB.UnloadModel(board);
            RB.CloseWindow();
        }
    }
}