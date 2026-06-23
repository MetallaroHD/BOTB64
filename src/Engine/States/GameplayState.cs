using BOTB64.Graphics.G3D;
using BOTB64.Runtime;
using BOTB64.Entities;
using RB = Raylib_cs.Raylib;
using RL = Raylib_cs;
using BOTB64.Graphics.UI;
using BOTB64.Entities.DTOs;

namespace BOTB64.Engine.States
{
    public class GameplayState : IGameState
    {
        public GameInitializer Initer;

        private Game Game = new();
        private Viewport Viewport = new();
        private DebugGameOverlayScreen Screen = new();

        public void OnEnter()
        {
            Game.Initialize(Initer);
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

            Viewport.End();

            (int q, int r) = HexAlgo.WorldToHex(Viewport.GetMouseXZ());
            Screen.PosLabel.Text = q.ToString() + ", " + r.ToString();
            Screen.FPSLabel.Text = RB.GetFPS().ToString();

            Screen.Draw();
        }
    }
}