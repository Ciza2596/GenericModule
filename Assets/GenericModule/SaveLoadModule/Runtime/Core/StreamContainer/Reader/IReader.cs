namespace SaveLoadModule
{
    public interface IReader
    {
        T Read<T>(string key);

        void Dispose();
    }
}