using BOTB64.Runtime;

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