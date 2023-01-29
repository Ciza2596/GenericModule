using System;
using System.Collections.Generic;

namespace DataType.Implement
{
    public interface IDataTypeControllerInstaller
    {
        public void Install(Dictionary<Type, DataType> dataTypes, IDataTypeController dataTypeController,
            IReflectionHelper reflectionHelper);
    }
}