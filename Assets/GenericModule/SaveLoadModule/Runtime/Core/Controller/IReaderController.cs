namespace SaveLoadModule
{
    public interface IReaderController
    {
        T Read<T>(string key);
    }
}