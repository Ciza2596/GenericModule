
using System;
using System.Collections.Generic;

namespace DataType.Implement
{
    public interface IDataTypeInstaller
    {
        public void Install(Dictionary<Type, DataType> dataTypes, IReflectionHelper reflectionHelper);
    }
}
