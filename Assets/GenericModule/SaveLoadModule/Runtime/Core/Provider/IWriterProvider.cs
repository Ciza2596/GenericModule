namespace SaveLoadModule
{
    public interface IWriterProvider
    {
        public IWriter CreateWriter(ReferenceModes referenceMode, string fullPath, int bufferSize,
            bool isWriteHeaderAndFooter,
            System.Text.Encoding encoding);
    }
}