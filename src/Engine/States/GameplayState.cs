using BOTB64.Graphics.G3D;
using BOTB64.Runtime;
using System.Numerics;
using RB = Raylib_cs.Raylib;
using RL = Raylib_cs;

namespace BOTB64.Engine.States
{
    public class GameplayState : IGameState
    {
        private Game Game = new();
        private Viewport Viewport = new();
        private RL.Model board;

        public void OnEnter()
        {
            Game.Initialize();

            DataFile df = new DataFile("Levels\\Level1\\board.gltf");
            board = RB.LoadModel(df.Path);
            for (int i = 0; i < board.MaterialCount; i++)
            {
                unsafe
                {
                    board.Materials[i].Shader = ShaderManager.WS.GetShader();
                }
            }
        }

        public void OnExit()
        {
            Game.Unload();
            RB.UnloadModel(board);
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
            RB.BeginShaderMode(ShaderManager.WS.GetShader());

            Game.Render();
            RB.DrawModel(board, Vector3.Zero, 1.0f, RL.Color.White);

            RB.EndShaderMode();

            Viewport.End();

            //Screen.Draw(); //update the ui
        }
    }
}