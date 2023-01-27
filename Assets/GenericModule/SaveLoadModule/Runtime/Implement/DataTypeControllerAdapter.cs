using System;
using DataType;
using DataType.Implement;

namespace SaveLoadModule.Implement
{
    public class DataTypeControllerAdapter : IDataTypeController
    {
        private readonly DataTypeController _dataTypeController;

        public DataTypeControllerAdapter(IReflectionHelperInstaller reflectionHelperInstaller)
        {
            _dataTypeController = new DataTypeController(new ReflectionHelper(reflectionHelperInstaller, this),
                new IDataTypeControllerInstaller[]
                {
                    new PrimitiveDataTypeControllerInstaller(),
                    new UnityDataTypeControllerInstaller()
                });
        }

        public DataType.DataType GetOrCreateDataType(Type key) => _dataTypeController.GetOrCreateDataType(key);
    }
}