using RL = Raylib_cs;

namespace BOTB64.Graphics
{
    public class Color
    {
        RL.Color RLColor = RL.Color.Black;

        public Color(RL.Color col)
        {
            RLColor = col;
        }

        public RL.Color Get()
        {
            return RLColor;
        }
    }
}
