
namespace SaveLoadModule.Implement
{
    public class JsonReaderController: IReaderController
    {
        //private variable
        private readonly IJsonReaderControllerConfig _jsonReaderControllerConfig;
        
        
        
        //public method
        public JsonReaderController(IJsonReaderControllerConfig jsonReaderControllerConfig)
        {
            _jsonReaderControllerConfig = jsonReaderControllerConfig;
        }


        public T Read<T>(string key) => throw new System.NotImplementedException();
    }
}