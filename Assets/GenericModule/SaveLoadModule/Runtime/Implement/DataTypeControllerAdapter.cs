using DataType;

namespace SaveLoadModule
{
    public class DataTypeControllerAdapter : IDataTypeController
    {
        private readonly DataTypeController _dataTypeController = new DataTypeController();
    }
}