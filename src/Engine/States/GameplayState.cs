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
        private Game Game = new();
        private Viewport Viewport = new();
        private DebugGameOverlayScreen Screen = new();

        ModelInstance dude = new ModelInstance(AssetManager.GetModel("Characters\\Dummy\\GenericCharacter.gltf"));

        public void OnEnter()
        {
            Game.Initialize(new GameInitializer
            {
                LevelInfo = new LevelDTO //we would read these from the json
                {
                    ModelURI = "Levels\\Level1\\Board.gltf",
                    ScriptURI = "Levels\\Level1\\board.b64m"
                },
                BlueTeam = new List<CharacterDTO>(),
                RedTeam = new List<CharacterDTO>()
            });
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

            dude.Draw();

            Viewport.End();

            (int q, int r) = HexAlgo.WorldToHex(Viewport.GetMouseXZ());
            Screen.PosLabel.Text = q.ToString() + ", " + r.ToString();
            Screen.FPSLabel.Text = RB.GetFPS().ToString();

            Screen.Draw();
        }
    }
}