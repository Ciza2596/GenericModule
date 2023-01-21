namespace SaveLoadModule
{
    public interface IWriterProvider
    {
        public IWriter CreateWriter(string fullPath, int bufferSize);
    }
}