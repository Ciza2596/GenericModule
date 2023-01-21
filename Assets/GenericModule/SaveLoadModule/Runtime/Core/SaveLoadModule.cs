namespace SaveLoadModule
{
    public class SaveLoadModule
    {
        //private variable
        private readonly IWriterController _writerController;
        private readonly IReaderController _readerController;


        //public method
        public SaveLoadModule(IWriterController writerController, IReaderController readerController)
        {
            _writerController = writerController;
            _readerController = readerController;
        }


        public void Save<T>(string key, T data, string path)
        {
            _writerController.Write<T>(key, data);
            _writerController.Save();
        }

        public T Load<T>(string key, string path) =>
            _readerController.Read<T>(key);
    }
}