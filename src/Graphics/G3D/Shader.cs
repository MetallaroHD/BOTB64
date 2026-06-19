using BOTB64.Runtime;
using RL = Raylib_cs;
using RB = Raylib_cs.Raylib;
using System.Security.Cryptography.X509Certificates;

namespace BOTB64.Graphics.G3D
{
    public class Shader : IDisposable
    {
        private RL.Shader RLShader;
        private bool Valid;
        private bool Disposed;

        public string VertexPath { get; }
        public string FragmentPath { get; }

        public RL.Shader Handle => RLShader;

        public Shader(string vertexFile, string fragmentFile)
        {
            VertexPath = vertexFile;
            FragmentPath = fragmentFile;

            DataFile vf = new DataFile(vertexFile);
            DataFile ff = new DataFile(fragmentFile);

            if (!vf.Exists())
            {
                Console.WriteLine($"[Shader] Vertex file not found: {vertexFile}");
                return;
            }
            if (!ff.Exists())
            {
                Console.WriteLine($"[Shader] Fragment file not found: {fragmentFile}");
                return;
            }

            RLShader = RB.LoadShader(vf.Path, ff.Path);

            // Raylib returns id=0 on failure, never a null struct
            if (RLShader.Id == 0)
            {
                Console.WriteLine($"[Shader] Raylib failed to compile: {vertexFile} / {fragmentFile}");
                return;
            }

            Valid = true;
        }

        public bool IsValid() => Valid && !Disposed;

        // Returns false if invalid — lets callers guard without try/catch
        public bool TryGet(out RL.Shader shader)
        {
            shader = RLShader;
            return IsValid();
        }

        public void Unload()
        {
            if (Valid && !Disposed)
                RB.UnloadShader(RLShader);
            Disposed = true;
            Valid = false;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (Disposed)
                return;

            if (Valid)
                RB.UnloadShader(RLShader);

            Disposed = true;
            Valid = false;
        }
    }
}