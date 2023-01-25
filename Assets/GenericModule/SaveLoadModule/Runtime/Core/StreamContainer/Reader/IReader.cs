namespace SaveLoadModule
{
    public interface IReader
    {
        public IReaderPropertyEnumerator Properties { get; }
        public IReaderRawEnumerator Raws { get; }

        T Read<T>(string key);
    }
}