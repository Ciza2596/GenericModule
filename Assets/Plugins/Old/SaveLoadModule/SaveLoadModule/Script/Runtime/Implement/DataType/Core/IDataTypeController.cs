
using System;

namespace DataType
{
    public interface IDataTypeController
    {
        BaseDataType GetOrCreateDataType(Type key);
    }
}
