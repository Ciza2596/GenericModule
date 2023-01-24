
using System;

namespace SaveLoadModule.Implement
{
    public interface IDataTypeController
    {
        DataType.DataType GetOrCreateDataType(Type key);
    }
}
