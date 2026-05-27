using RL = Raylib_cs;
using RB = Raylib_cs.Raylib;
using RM = Raylib_cs.Raymath;

using System.Diagnostics;
using System.Numerics;

RB.InitWindow(1280, 720, "BOTB64");
RB.SetTargetFPS(60);

Stopwatch clock = new Stopwatch();
clock.Start();

RL.Model playerModel = RB.LoadModel("G:\\Dev\\BOTB64\\data\\tizio.obj");

Vector3 playerPos = new Vector3(0, 1, 0);
float playerRot = 0;

RL.Shader shader = RB.LoadShader(
    "G:\\Dev\\BOTB64\\data\\inverse_square.vs",
    "G:\\Dev\\BOTB64\\data\\inverse_square.fs"
);

unsafe
{
    for (int i = 0; i < playerModel.MaterialCount; i++)
    {
        playerModel.Materials[i].Shader = shader;
    }
}

RL.Camera3D camera = new RL.Camera3D();
camera.Up = new Vector3(0, 1, 0);
camera.FovY = 45.0f;
camera.Projection = RL.CameraProjection.Perspective;

int lightPosLoc = RB.GetShaderLocation(shader, "lightPos");
int lightColorLoc = RB.GetShaderLocation(shader, "lightColor");
int viewPosLoc = RB.GetShaderLocation(shader, "viewPos");

Vector3 lightPos = new Vector3(2, 4, 2);
Vector3 lightColor = new Vector3(0, 1, 1);

Random rng = new Random();

while (!RB.WindowShouldClose())
{
    float dt = (float)clock.Elapsed.TotalSeconds;
    clock.Restart();

    //InputManager.HandleInput();
    if (RB.IsKeyDown(RL.KeyboardKey.W))
        playerPos.Z -= 0.2f;
    if (RB.IsKeyDown(RL.KeyboardKey.S))
        playerPos.Z += 0.2f;
    if (RB.IsKeyDown(RL.KeyboardKey.Q))
        playerPos.X -= 0.2f;
    if (RB.IsKeyDown(RL.KeyboardKey.E))
        playerPos.X += 0.2f;
    if (RB.IsKeyDown(RL.KeyboardKey.A))
        playerRot -= 2f;
    if (RB.IsKeyDown(RL.KeyboardKey.D))
        playerRot += 2f;

    //Engine.Update(InputManager.CommandQueue, dt);
    camera.Position = playerPos + new Vector3(5, 5, 5);
    camera.Target = playerPos;
    lightPos = camera.Position;

    //Graphics.Draw(Engine.GameState);
    RB.SetShaderValue(shader, lightPosLoc, lightPos, RL.ShaderUniformDataType.Vec3);
    RB.SetShaderValue(shader, lightColorLoc, lightColor, RL.ShaderUniformDataType.Vec3);
    RB.SetShaderValue(shader, viewPosLoc, camera.Position, RL.ShaderUniformDataType.Vec3);

    RB.BeginDrawing();
    RB.ClearBackground(RL.Color.White);
    RB.BeginMode3D(camera);

    RB.DrawPlane(
        new Vector3(0, 0, 0),
        new Vector2(10, 10),
        RL.Color.LightGray
    );
    RB.DrawModelEx(
        playerModel,
        playerPos,
        new Vector3(0, 1, 0),
        playerRot,
        new Vector3(0.1f, 0.1f, 0.1f),
        RL.Color.White
    );

    RB.EndMode3D();
    RB.DrawFPS(10, 10);
    RB.EndDrawing();
}

RB.UnloadShader(shader);
RB.UnloadModel(playerModel);
RB.CloseWindow();