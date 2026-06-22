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

        ModelInstance dude = new ModelInstance(AssetManager.GetModel("Characters\\Dummy\\GenericCharacter.gltf"));

        public void OnEnter()
        {
            Game.Initialize();
            Level = Level.Load("Levels\\Level1\\board.b64m");
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
            dude.Draw();

            Viewport.End();

            for (int i = 0; i < Level.LevelBoard.TileCountRow; i++)
            {
                for (int j = 0; j < Level.LevelBoard.TileCountCol; j++)
                {
                    Level.LevelBoard.RestoreColor(i, j);
                }
            }

            (int q, int r) = HexAlgo.WorldToHex(Viewport.GetMouseXZ());
            List<(int, int)> ls = HexAlgo.Beam(0, 0, q, r);
            foreach (var p in ls)
            {
                (int x, int y) = HexAlgo.HexToIndex(p.Item1, p.Item2, Level.LevelBoard.TileCountRow, Level.LevelBoard.TileCountCol);
                if(Level.LevelBoard.IsValidIndex(x, y))
                    Level.LevelBoard.Tiles[x][y].SetColor(RL.Color.Yellow);
            }

            //(int row, int col) = Level.LevelBoard.HexToIndex(q, r);
            //if (Level.LevelBoard.IsValidIndex(row, col))
            //    Level.LevelBoard.Tiles[row][col].SetColor(RL.Color.Yellow);

            Screen.PosLabel.Text = q.ToString() + ", " + r.ToString();
            Screen.FPSLabel.Text = RB.GetFPS().ToString();

            Screen.Draw();
        }
    }
}