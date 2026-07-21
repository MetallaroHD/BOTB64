using BOTB64.Shared.Files;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Loaders;

namespace BOTB64.Runtime
{
    public class ArchiveScriptLoader : ScriptLoaderBase
    {
        private readonly Func<string, string> _loadScript;

        public ArchiveScriptLoader(Func<string, string> loadScript)
        {
            _loadScript = loadScript;
        }

        public override object LoadFile(string file, Table globalContext)
        {
            return _loadScript(file);
        }

        public override bool ScriptFileExists(string name)
        {
            try
            {
                _loadScript(name);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public override string ResolveModuleName(string modname, Table globalContext)
        {
            return modname;
        }
    }
}
