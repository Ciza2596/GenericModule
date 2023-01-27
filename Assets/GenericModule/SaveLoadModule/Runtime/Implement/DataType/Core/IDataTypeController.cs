
using System;

namespace DataType
{
    public interface IDataTypeController
    {
        DataType GetOrCreateDataType(Type key);
    }
}
