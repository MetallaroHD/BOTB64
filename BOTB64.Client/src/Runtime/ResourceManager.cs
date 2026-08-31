using BOTB64.Graphics.G3D;
using BOTB64.Shared.Files;
using BOTB64.Shared.DTOs;
using Raylib_cs;
using System.IO.Compression;

namespace BOTB64.Runtime;

public static class ResourceManager
{
    // 3d model cache, saved by path
    private static readonly Dictionary<string, ModelAsset> Models = new();
    private static readonly Dictionary<string, Texture2D> Textures = new();

    //@TODO: caches for vfx

    private static readonly string TempAssetRoot = Path.Combine(Path.GetTempPath(), "BOTB64", "Assets");

    public static void Initialize()
    {
        ClearCache();

        Directory.CreateDirectory(TempAssetRoot);
    }

    public static bool Exists(string uri)
    {
#if DEVELOPMENT
        return File.Exists(uri);
#else
        return ResourceArchive.Exists(uri);
#endif
    }

    public static Texture2D GetAuraIcon(int id)
    {
        AuraDTO? auraD = DatabaseFileManager.Auras.FirstOrDefault(s => s.ID == id);

        if (auraD == null)
            return default;

        return LoadTexture(auraD.IconURI);
    }

    public static Texture2D GetSpellIcon(int id)
    {
        SpellDTO? spellD = DatabaseFileManager.Spells.FirstOrDefault(s => s.ID == id);

        if (spellD == null)
            return default;

        return LoadTexture(spellD.IconURI);
    }

    public static void ClearCache()
    {
        ClearModels();
        foreach (var icon in Textures.Values)
            Raylib.UnloadTexture(icon);
        Textures.Clear();
    }

    public static void ClearModels()
    {
        if (Directory.Exists(TempAssetRoot))
            Directory.Delete(TempAssetRoot, true);

        foreach (var model in Models.Values)
            model.Dispose();
        Models.Clear();
    }

    public static string ReadText(string uri)
    {
        var file = new DataFile(uri);

#if DEVELOPMENT
        return File.ReadAllText(file.AbsPath);
#else
        return ResourceArchive.ReadAllText(file.AbsPath);
#endif
    }

    public static byte[] ReadBytes(string uri)
    {
        var file = new DataFile(uri);

#if DEVELOPMENT
        return File.ReadAllBytes(file.AbsPath);
#else
        return ResourceArchive.ReadAllBytes(file.AbsPath);
#endif
    }

    public static Texture2D LoadTexture(string uri)
    {
        if(Textures.TryGetValue(uri, out Texture2D texture))
            return texture;

        byte[] data = ReadBytes(uri);

        if (data.Length == 0)
            return default;

        Image image = Raylib.LoadImageFromMemory(GetExtension(uri), data);
        Texture2D ldtxt = Raylib.LoadTextureFromImage(image);
        Textures.Add(uri, ldtxt);

        Raylib.UnloadImage(image);

        return ldtxt;
    }

    public static Raylib_cs.Shader LoadShader(string vertexURI, string fragmentURI)
    {
#if DEVELOPMENT

        return Raylib.LoadShader(vertexURI,fragmentURI);

#else
        string vs = ReadText(vertexURI);
        string fs = ReadText(fragmentURI);

        return Raylib.LoadShaderFromMemory(vs, fs);
#endif
    }

    private static string GetExtension(string uri)
    {
        return Path.GetExtension(uri).ToLowerInvariant();
    }

    public static Model LoadModel(string uri)
    {
#if DEVELOPMENT
        return Raylib.LoadModel(new DataFile(uri).AbsPath);
#else
        string folder = ExtractModelFolder(uri);
        string gltf = Path.Combine(folder, Path.GetFileName(folder));

        return Raylib.LoadModel(gltf);
#endif
    }

    private static void ExtractEntry(ZipArchiveEntry entry, string destination)
    {
        string? directory = Path.GetDirectoryName(destination);

        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);

        using Stream input = entry.Open();
        using FileStream output =
            File.Create(destination);

        input.CopyTo(output);
    }

    public static string ExtractModelFolder(string modelURI)
    {
        string uniqueID = modelURI;
        string outputFolder =Path.Combine(TempAssetRoot, uniqueID);
        Directory.CreateDirectory(outputFolder);
        string directory = Path.GetDirectoryName(modelURI)!.Replace('\\', '/');

        foreach (var entry in ResourceArchive.Entries)
        {
            string entryPath = entry.FullName.Replace('\\', '/');

            if (!entryPath.StartsWith(directory + "/", StringComparison.OrdinalIgnoreCase))
                continue;

            string extension = Path.GetExtension(entryPath).ToLowerInvariant();

            if (extension is ".gltf" or ".bin" or ".png" or ".glb")
            {
                string relative = entryPath.Substring(directory.Length).TrimStart('/');
                string destination = Path.Combine(outputFolder, relative);

                ExtractEntry(entry, destination);
            }
        }

        return outputFolder;
    }

    public static ModelAsset GetModel(string path, ModelPurpose purpose)
    {
        if (!Models.TryGetValue(path, out var model))
        {
            model = new ModelAsset(path, purpose);
            Models[path] = model;
        }

        return model;
    }

    // Re-applies each cached model's texture filter for the current Settings.Scale -
    // call this whenever Scale changes at runtime (fullscreen toggle, settings screen),
    // since already-loaded models otherwise keep whatever filter they were created with.
    public static void RefreshTextureFilters()
    {
        foreach (var model in Models.Values)
            model.RefreshTextureFilter();
    }
}