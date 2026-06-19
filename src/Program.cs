using BOTB64.Runtime;
using BOTB64.Graphics;

namespace BOTB64
{
    public class Program
    {
        public static void Main()
        {
            Graphics.Graphics.Initialize(1280, 720, "BOTB64");
            Engine.Engine.Initialize();

            if (!Graphics.Graphics.CompileShader("Shaders\\shader.vs", "Shaders\\shader.fs"))
                Console.WriteLine("Warning: shader compilation failed, using default.");

            while (!InputManager.ShouldClose())
                Engine.Engine.Update();

            Graphics.Graphics.Unload();
        }
    }
}