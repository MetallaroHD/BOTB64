using BOTB64.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOTB64.Graphics.G3D
{
    public static class AssetManager
    {
        private static readonly Dictionary<string, ModelAsset> Models = new();

        public static ModelAsset GetModel(string path, ModelPurpose purpose)
        {
            if(!Models.TryGetValue(path, out var model))
            {
                model = new ModelAsset(path, purpose);
                Models[path] = model;
            }

            return model;
        }

        public static void UnloadAll()
        {
            foreach (var model in Models.Values)
                model.Dispose();
            Models.Clear();
        }
    }
}
