namespace CizaSaveLoadModule
{
    public interface IReaderProvider
    {
        IReader CreateReader(string fullPath, int bufferSize);
    }
}