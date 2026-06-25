using BOTB64.Entities.DTOs;
using BOTB64.Graphics.G3D;
using BOTB64.Runtime;
using System.Numerics;

namespace BOTB64.Entities
{
    public struct GameInitializer
    {
        public LevelDTO Level;
        public List<CharacterDTO> BlueTeam;
        public List<CharacterDTO> RedTeam;
    }

    public class Game
    {
        private Level Level = new Level();
        private List<Character> Characters = new();
        
        public Board GetBoard() => Level.LevelBoard;

        public void Initialize(GameInitializer lI)
        {
            Level = Level.Load(lI.Level.ScriptURI, lI.Level.ModelURI);
        }

        public void Update(float dt)
        {

        }

        public void Render()
        {
            Level.LevelBoard.Draw();
            foreach (var character in Characters) 
                character.Draw();
        }

        public void Unload() 
        {
            AssetManager.UnloadAll();
        }
    }
}