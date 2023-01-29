namespace SaveLoadModule
{
    public interface IWriterProvider
    {
        public IWriter CreateWriter(string fullPath, int bufferSize,
            bool isWriteHeaderAndFooter,
            System.Text.Encoding encoding);
    }
}