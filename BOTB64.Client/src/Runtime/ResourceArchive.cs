using System.Collections.ObjectModel;
using System.IO.Compression;
using System.Security.Cryptography;

namespace BOTB64.Runtime;

public static class ResourceArchive
{
    private static byte[] _key = Convert.FromHexString("3661bcd483c06d9ba26086584e0f62296f46eaa5ff4ee42f039297ec4ca38e2b");

    private static ZipArchive? _archive;
    public static ReadOnlyCollection<ZipArchiveEntry>? Entries => _archive?.Entries;

    public static void Initialize(string archivePath)
    {
        if (File.Exists(archivePath))
            _archive = OpenArchive(archivePath, _key);
    }

    public static string ToArchivePath(string absolutePath)
    {
        string root = DataFile.DataDir;

        absolutePath = System.IO.Path.GetRelativePath(root,absolutePath);

        return absolutePath.Replace('\\', '/');
    }

    public static bool Exists(string absolutePath)
    {
        if (_archive == null)
            return File.Exists(absolutePath);

        return _archive.GetEntry(ToArchivePath(absolutePath)) != null;
    }

    public static string ReadAllText(string absolutePath)
    {
        if (_archive == null)
            return File.ReadAllText(absolutePath);

        string archivePath = ToArchivePath(absolutePath);

        var entry = _archive.Entries.FirstOrDefault(e => e.FullName.Replace('\\', '/').Equals(archivePath, StringComparison.OrdinalIgnoreCase));

        if (entry == null)
            return "";

        using var reader = new StreamReader(entry.Open());
        return reader.ReadToEnd();
    }

    public static byte[] ReadAllBytes(string absolutePath)
    {
        if (_archive == null)
            return File.ReadAllBytes(absolutePath);

        string archivePath = ToArchivePath(absolutePath);

        var entry = _archive.Entries.FirstOrDefault(e => e.FullName.Replace('\\', '/').Equals(archivePath, StringComparison.OrdinalIgnoreCase));

        if (entry == null)
            return [];

        using var stream = entry.Open();
        using MemoryStream ms = new();
        stream.CopyTo(ms);
        return ms.ToArray();
    }

    public static string[] ReadAllLines(string absolutePath)
    {
        return ReadAllText(absolutePath)
            .Split(Environment.NewLine, StringSplitOptions.None);
    }

    public static ZipArchive OpenArchive(string file, byte[] key)
    {
        FileStream fs = File.OpenRead(file);

        byte[] iv = new byte[16];

        fs.Read(iv, 0, iv.Length);

        Aes aes = Aes.Create();

        aes.Key = key;
        aes.IV = iv;

        CryptoStream crypto = new(fs, aes.CreateDecryptor(), CryptoStreamMode.Read);

        return new ZipArchive(crypto, ZipArchiveMode.Read);
    }
}