namespace SaveLoadModule.Implement
{
    public class JsonWriterController: IWriterController
    {

        //private variable
        private readonly IStreamProvider _streamProvider;
        private readonly IJsonWriterControllerConfig _jsonWriterControllerConfig;
        
        
        
        //public method
        public JsonWriterController(IStreamProvider streamProvider,
            IJsonWriterControllerConfig jsonWriterControllerConfig)
        {
            _streamProvider = streamProvider;
            _jsonWriterControllerConfig = jsonWriterControllerConfig;
        }
        

        public void Write<T>(string key, T data)
        {
            
        }

        public void Save()
        {
        }
    }
}