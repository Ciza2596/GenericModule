using System;
using System.Collections.Generic;

namespace DataType.Implement
{
    public interface IDataTypeControllerInstaller
    {
        public void Install(Dictionary<Type, BaseDataType> dataTypes, IDataTypeController dataTypeController,
            IReflectionHelper reflectionHelper);
    }
}