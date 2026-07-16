using System.Text.Json;

namespace BOTB64.Shared.Files
{
    public class JsonDataFile<T> : IDataFileRW<T>
    {
        private static readonly JsonSerializerOptions Options = new()
        {
            WriteIndented = true
        };

        public T Read(DataFile df)
        {
            string json = df.ReadAll();
            return JsonSerializer.Deserialize<T>(json, Options);
        }

        public bool TryRead(DataFile df, out T data)
        {
            data = default;
            if (!df.Exists())
                return false;

            try
            {
                data = Read(df);
                return true;
            }
            catch (JsonException)
            {
                return false;
            }
        }

        public void Write(DataFile df, T data)
        {
            string json = JsonSerializer.Serialize(data, Options);
            df.WriteAll(json);
        }

        public List<T> DeserializeAll(DataFile df)
        {
            string json = df.ReadAll();
            return JsonSerializer.Deserialize<List<T>>(json, Options);
        }
    }
}
