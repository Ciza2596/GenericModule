namespace SaveLoadModule
{
    public interface IReaderProvider
    {
        IReader CreateReader(string path);
    }
}