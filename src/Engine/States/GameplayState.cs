using BOTB64.Graphics.G3D;
using BOTB64.Runtime;
using BOTB64.Entities;
using System.Numerics;
using RB = Raylib_cs.Raylib;
using RL = Raylib_cs;
using BOTB64.Graphics.UI;
using System.Security.AccessControl;

namespace BOTB64.Engine.States
{
    public class GameplayState : IGameState
    {
        private Game Game = new();
        private Viewport Viewport = new();
        private Level Level;
        private DebugGameOverlayScreen Screen = new();

        private ModelInstance charac;

        public void OnEnter()
        {
            Game.Initialize();
            Level = Level.Load("Levels\\Level1\\board.b64m");
            ModelAsset bro = AssetManager.GetModel("Characters\\Dummy\\character.gltf");
            charac = new ModelInstance(bro);
        }

        public void OnExit()
        {
            Game.Unload();
        }

        public void Update(float dt)
        {
            Game.Update(dt);
            Viewport.Update(dt);
        }

        public void Render()
        {
            Viewport.Begin();

            ShaderManager.Update();

            Game.Render();
            Level.LevelBoard.Draw();

            Viewport.End();

            charac.Draw();

            //RL.Ray ray = RB.GetScreenToWorldRay(RB.GetMousePosition(),Viewport.Camera.Camera);
            //Vector3 mouseWorld = Vector3.Zero;
            //if (MathF.Abs(ray.Direction.Y) > 0.0001f)
            //{
            //    float t = -ray.Position.Y / ray.Direction.Y;

            //    mouseWorld = ray.Position + ray.Direction * t;
            //}
            //Screen.FPSLabel.Text = mouseWorld.X.ToString() + ", " + mouseWorld.Y.ToString() + ", " + mouseWorld.Z.ToString();

            Screen.FPSLabel.Text = RB.GetFPS().ToString();

            Screen.Draw();
        }
    }
}