using BOTB64.Core;
using BOTB64.Graphics;
using BOTB64.Engine;
using System.Numerics;
using RB = Raylib_cs.Raylib;
using RL = Raylib_cs;

namespace BOTB64
{
    public class Program
    {
        public static void Main()
        {
            Engine.Engine.Initialize();
            Graphics.Graphics.Initialize();
            if (!Graphics.Graphics.CompileShader("Shaders\\shader.vs", "Shaders\\shader.fs"))
            {
                Console.WriteLine("Failed to compile shader.");
            }
            if (!Loop()) ;
                //Handle error codes
            Close();
        }

        public static bool Loop()
        {
            while (!InputManager.ShouldClose())
            {
                Engine.Engine.Update();
                Graphics.Graphics.Update(Engine.Engine.Elapsed());
            }
            return true;
        }

        public static void Close()
        {
            Graphics.Graphics.Unload();
        }
    }
}