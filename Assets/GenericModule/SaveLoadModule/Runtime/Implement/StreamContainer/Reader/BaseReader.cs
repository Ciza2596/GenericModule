namespace SaveLoadModule.Implement
{
    public abstract class BaseReader: IReader
    {
        public T Read<T>(string key) => throw new System.NotImplementedException();
    }
}