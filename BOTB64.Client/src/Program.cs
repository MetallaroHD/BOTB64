using BOTB64.Runtime;

namespace BOTB64
{
    public class Program
    {
        public static void Main()
        {
            if (Version.Expires < DateTime.Now)
                return;

            if (!DataFile.DirectoryExists())
                return;

            ResourceManager.Initialize();
            ResourceArchive.Initialize(DataFile.DataDir);
            Graphics.Graphics.Initialize(1280, 720, "BOTB64");
            Engine.Engine.Initialize();

            while (!InputManager.ShouldClose())
            {
                InputManager.NewFrame();
                Engine.Engine.Update();
                Engine.Engine.Render();
            }

            Graphics.Graphics.Unload();
        }
    }
}