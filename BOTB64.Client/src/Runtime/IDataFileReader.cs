namespace BOTB64.Runtime
{
    public interface IDataFileReader<T>
    {
        T Read(DataFile df);

        bool TryRead(DataFile df, out T Data);
    }

    public interface IDataFileWriter<T>
    {
        void Write(DataFile df, T data);
    }

    public interface IDataFileRW<T> : IDataFileReader<T>, IDataFileWriter<T>
    {

    }
}
