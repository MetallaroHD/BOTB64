using BOTB64.Entities;

namespace BOTB64.Runtime
{
    public class BOTBDatafile<T> : IDataFileRW<T> where T : IReadable
    {
        protected const string HeaderTag = "";

        public virtual T Read(DataFile df)
        {
            throw new NotImplementedException();
        }
        public virtual bool TryRead(DataFile df, out T data)
        {
            throw new NotImplementedException();
        }
        public virtual void Write(DataFile df, T r)
        {
            throw new NotImplementedException();
        }

        protected static bool TryParseHeader(string line)
        {
            if (!line.StartsWith(HeaderTag, StringComparison.Ordinal))
                return false;

            return true;
        }

        protected static int SkipBlankAndComments(string[] lines, int i)
        {
            while (i < lines.Length)
            {
                string t = lines[i].Trim();
                if (t.Length > 0 && !t.StartsWith("#", StringComparison.Ordinal))
                    break;
                i++;
            }

            return i;
        }
    }
}
