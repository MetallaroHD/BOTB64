using BOTB64.Engine;

namespace BOTB64.Entities
{
    public class Turn
    {
        private Game Parent;

        public int Number { get; set; }
        public Character ActiveCharacter { get; set; }
        public bool HasMoved { get; set; }

        public Turn(int number, Character character, Game parent)
        {
            Parent = parent;
            Number = number;
            ActiveCharacter = character;
        }
    }
}
