using BOTB64.Graphics.G3D;
using BOTB64.Runtime;
using BOTB64.Entities;
using RB = Raylib_cs.Raylib;
using RL = Raylib_cs;
using BOTB64.Graphics.UI;
using BOTB64.Entities.DTOs;
using BOTB64.Engine.Actions;

namespace BOTB64.Engine.States
{
    public class GameplayState : IGameState
    {
        public GameInitializer Initer;

        private Game Game = new();
        private Viewport Viewport = new();
        private DebugGameOverlayScreen Screen = new();

        private IAction? CurrentAction;

        public void OnEnter()
        {
            Game.Initialize(Initer);
            Targeter.SetBoard(Game.GetBoard());
            ChangeAction(new DefaultAction(this));
        }

        public void OnExit()
        {
            Game.Unload();
        }

        public void Update(float dt)
        {
            CurrentAction?.Update();
            Game.Update(dt);
            Viewport.Update(dt);
            Screen.Update(dt);
            FloatingTextManager.Update(dt);
        }

        public void Render()
        {
            Viewport.Begin();
            ShaderManager.Update();
            Game.Render();
            Viewport.End();

            // DEBUG
            Hex mouseCast = HexAlgo.WorldToHex(Viewport.GetMouseXZ());
            Screen.PosLabel.Text = mouseCast.Q.ToString() + ", " + mouseCast.R.ToString();
            Screen.FPSLabel.Text = RB.GetFPS().ToString();
            ////////

            FloatingTextManager.Draw(Viewport);
            Screen.Draw();
        }

        public void ChangeAction(IAction action)
        {
            CurrentAction?.Exit();
            CurrentAction = action;
            CurrentAction?.Enter();
        }

        public Hex GetMouseAxial()
        {             
            return HexAlgo.WorldToHex(Viewport.GetMouseXZ());
        }
    }
}