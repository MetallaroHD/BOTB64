using BOTB64.Entities.DTOs;
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
        
        public void Initialize(GameInitializer lI)
        {
            Level = Level.Load(lI.Level.ScriptURI, lI.Level.ModelURI);
        }

        public void Update(float dt)
        {

            // collision, spawning, etc.
        }

        public void Render()
        {
            Level.LevelBoard.Draw();
        }

        public void Unload() 
        {
            /* free any loaded assets */ 
        }
    }
}