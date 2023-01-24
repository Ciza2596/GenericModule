using System;
using DataType.Implement;

namespace SaveLoadModule.Implement
{
    public class DataTypeControllerAdapter : IDataTypeController
    {
        private readonly DataTypeController _dataTypeController = new DataTypeController(new ReflectionHelper(), new IDataTypeInstaller[]
        {
            new PrimitiveDataTypeInstaller()
        });
        public DataType.DataType GetOrCreateDataType(Type key) => _dataTypeController.GetOrCreateDataType(key);
    }
}