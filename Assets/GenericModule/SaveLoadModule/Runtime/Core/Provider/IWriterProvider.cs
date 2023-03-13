namespace CizaSaveLoadModule
{
    public interface IWriterProvider
    {
        public IWriter CreateWriter(string fullPath, int bufferSize,
            System.Text.Encoding encoding);
    }
}