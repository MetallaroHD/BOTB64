using BOTB64.Packer;

byte[] key = Convert.FromHexString("3661bcd483c06d9ba26086584e0f62296f46eaa5ff4ee42f039297ec4ca38e2b");

string solutionRoot = Path.GetFullPath(
    Path.Combine(
        AppContext.BaseDirectory,
        @"..\..\..\.."
    )
);
string input = Path.Combine(
    solutionRoot,
    "BOTB64.Client",
    "data.zip"
);
string output = Path.Combine(
    solutionRoot,
    "BOTB64.Client",
    "data.b64"
);

AesArchive.EncryptFile(
    input,
    output,
    key
);

Console.WriteLine("Packed!");