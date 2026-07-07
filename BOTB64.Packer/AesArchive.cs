using System.IO.Compression;
using System.Security.Cryptography;

namespace BOTB64.Packer;

public static class AesArchive
{
    public static void EncryptFile(
        string input,
        string output,
        byte[] key)
    {
        string normalizedZip =
            Path.Combine(
                Path.GetDirectoryName(input)!,
                "normalized.zip"
            );


        NormalizeZip(
            input,
            normalizedZip
        );


        using Aes aes = Aes.Create();

        aes.Key = key;
        aes.GenerateIV();


        using FileStream outputStream =
            File.Create(output);


        outputStream.Write(
            aes.IV,
            0,
            aes.IV.Length
        );


        using CryptoStream crypto =
            new(
                outputStream,
                aes.CreateEncryptor(),
                CryptoStreamMode.Write
            );


        using FileStream inputStream =
            File.OpenRead(normalizedZip);


        inputStream.CopyTo(crypto);
    }


    private static void NormalizeZip(
        string input,
        string output)
    {
        if (File.Exists(output))
            File.Delete(output);


        using ZipArchive source =
            ZipFile.OpenRead(input);


        using ZipArchive destination =
            ZipFile.Open(
                output,
                ZipArchiveMode.Create
            );


        foreach (ZipArchiveEntry entry in source.Entries)
        {
            string normalized =
                entry.FullName
                    .Replace('\\', '/');


            ZipArchiveEntry newEntry =
                destination.CreateEntry(
                    normalized
                );


            using Stream inputStream =
                entry.Open();

            using Stream outputStream =
                newEntry.Open();

            inputStream.CopyTo(outputStream);
        }
    }
}