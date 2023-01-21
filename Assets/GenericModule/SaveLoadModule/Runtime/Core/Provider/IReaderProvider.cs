namespace SaveLoadModule
{
    public interface IReaderProvider
    {
        IReader CreateReader(string fullPath, int bufferSize);
    }
}