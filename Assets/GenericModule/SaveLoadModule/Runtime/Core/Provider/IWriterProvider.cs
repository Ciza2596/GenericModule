namespace SaveLoadModule
{
    public interface IWriterProvider
    {
        public IWriter CreateWriter(string path);
    }
}