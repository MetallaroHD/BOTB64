using BOTB64.Engine;
using BOTB64.Graphics;
using BOTB64.Graphics.UI;
using BOTB64.Runtime;
using BOTB64.Shared;

namespace BOTB64
{
    public class Program
    {
        public static void Main()
        {
            if (BOTBVersion.Expires < DateTime.Now)
                return;

            if (!DataFile.DirectoryExists())
                return;

            Settings.Load();
            UIRenderer.Update();
            ResourceManager.Initialize();
            ResourceArchive.Initialize(DataFile.DataDir);
            LuaEffectRunner.RegisterTypes();
            Graphics.Graphics.Initialize(1280, 720, "BOTB64");
            Engine.Engine.Initialize();

            while (!InputManager.ShouldClose())
            {
                WindowManager.Update();
                InputManager.NewFrame();
                Engine.Engine.Update();
                Engine.Engine.Render();
            }

            Graphics.Graphics.Unload();
            ResourceManager.ClearCache();
        }
    }
}