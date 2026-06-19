using System.Numerics;

namespace BOTB64.Engine.States
{
    public class Game
    {
        // characters, map, pickups, score, etc.

        public void Initialize()
        {
            //clear all and refill
        }

        public void Update(float dt)
        {

            // collision, spawning, etc.
        }

        public void Render()
        {
            //call render for each unit
        }

        public void Unload() 
        {
            /* free any loaded assets */ 
        }
    }
}