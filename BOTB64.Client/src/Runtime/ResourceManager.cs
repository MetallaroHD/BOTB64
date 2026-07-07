using Raylib_cs;
using System.IO.Compression;

namespace BOTB64.Runtime;

public static class ResourceManager
{
    private static readonly string TempAssetRoot =
    Path.Combine(
        Path.GetTempPath(),
        "BOTB64",
        "Assets"
    );


    public static void Initialize()
    {
        if (Directory.Exists(TempAssetRoot))
        {
            Directory.Delete(
                TempAssetRoot,
                true
            );
        }

        Directory.CreateDirectory(
            TempAssetRoot
        );
    }

    public static bool Exists(string uri)
    {
#if DEVELOPMENT
        return File.Exists(uri);
#else
        return ResourceArchive.Exists(uri);
#endif
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
        byte[] data = ReadBytes(uri);

        if (data.Length == 0)
            return default;


        Image image = Raylib.LoadImageFromMemory(
            GetExtension(uri),
            data
        );

        Texture2D texture = Raylib.LoadTextureFromImage(image);

        Raylib.UnloadImage(image);

        return texture;
    }


    public static Shader LoadShader(string vertexURI, string fragmentURI)
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
        return Path.GetExtension(uri)
            .ToLowerInvariant();
    }

    public static Model LoadModel(string uri)
    {
#if DEVELOPMENT
        return Raylib.LoadModel(new DataFile(uri).AbsPath);
#else

        string folder = ExtractModelFolder(uri);

        string gltf =
            Directory.GetFiles(
                folder,
                "*.gltf"
            )[0];

        return Raylib.LoadModel(gltf);

#endif
    }

    private static void ExtractEntry(
    ZipArchiveEntry entry,
    string destination)
    {
        string? directory =
            Path.GetDirectoryName(destination);

        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);


        using Stream input = entry.Open();
        using FileStream output =
            File.Create(destination);

        input.CopyTo(output);
    }

    public static string ExtractModelFolder(string modelURI)
    {
        string uniqueID =
            Guid.NewGuid().ToString();


        string outputFolder =
            Path.Combine(
                TempAssetRoot,
                uniqueID
            );


        Directory.CreateDirectory(outputFolder);


        string directory =
            Path.GetDirectoryName(modelURI)!
                .Replace('\\', '/');


        foreach (var entry in ResourceArchive.Entries)
        {
            string entryPath =
                entry.FullName.Replace('\\', '/');


            if (!entryPath.StartsWith(
                directory + "/",
                StringComparison.OrdinalIgnoreCase))
                continue;


            string extension =
                Path.GetExtension(entryPath)
                    .ToLowerInvariant();


            if (extension is ".gltf"
                or ".bin"
                or ".png")
            {
                string relative =
                    entryPath
                        .Substring(directory.Length)
                        .TrimStart('/');


                string destination =
                    Path.Combine(
                        outputFolder,
                        relative
                    );


                ExtractEntry(
                    entry,
                    destination
                );
            }
        }


        return outputFolder;
    }
}