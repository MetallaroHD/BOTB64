using BOTB64.Runtime;
using BOTB64.Graphics;
using BOTB64.Graphics.G3D;

namespace BOTB64
{
    public class Program
    {
        public static void Main()
        {
            Graphics.Graphics.Initialize(1280, 720, "BOTB64");
            Engine.Engine.Initialize();

            while (!InputManager.ShouldClose())
            {
                Engine.Engine.Update();
                Engine.Engine.Render();
            }

            Graphics.Graphics.Unload();
        }
    }
}