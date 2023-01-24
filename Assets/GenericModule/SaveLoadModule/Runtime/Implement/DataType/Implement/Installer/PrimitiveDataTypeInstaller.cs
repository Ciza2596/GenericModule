using System;
using System.Collections.Generic;
using DataType;

namespace SaveLoadModule
{
    public class PrimitiveDataTypeInstaller : IDataTypeInstaller
    {
        private Dictionary<Type, DataType.DataType> _dataTypes;

        public void Install(Dictionary<Type, DataType.DataType> dataTypes, IReflectionHelper reflectionHelper)
        {
            _dataTypes = dataTypes;

            var boolDataType = new BoolDataType();
            AddDataTypeToDataTypes(boolDataType, new BoolArrayDataType(boolDataType, reflectionHelper));

            var charDataType = new CharDataType();
            AddDataTypeToDataTypes(charDataType, new CharArrayDataType(charDataType, reflectionHelper));

            var longDataType = new LongDataType();
            AddDataTypeToDataTypes(longDataType, new LongArrayDataType(longDataType, reflectionHelper));

            var dateTimeDataType = new DateTimeDataType(longDataType);
            AddDataTypeToDataTypes(dateTimeDataType, new DateTimeArrayDataType(dateTimeDataType, reflectionHelper));

            var doubleDataType = new DoubleDataType();
            AddDataTypeToDataTypes(doubleDataType, new DoubleArrayDataType(doubleDataType, reflectionHelper));

            var floatDataType = new FloatDataType();
            AddDataTypeToDataTypes(floatDataType, new FloatArrayDataType(floatDataType, reflectionHelper));

            var intDataType = new IntDataType();
            AddDataTypeToDataTypes(intDataType, new IntArrayDataType(intDataType, reflectionHelper));

            var shortDataType = new ShortDataType();
            AddDataTypeToDataTypes(shortDataType, new ShortArrayDataType(shortDataType, reflectionHelper));

            var stringDataType = new StringDataType();
            AddDataTypeToDataTypes(stringDataType, new StringArrayDataType(stringDataType, reflectionHelper));
            
            _dataTypes = null;
        }


        private void AddDataTypeToDataTypes(DataType.DataType dataType, ArrayDataType arrayDataType)
        {
            _dataTypes.Add(dataType.Type, dataType);
            _dataTypes.Add(arrayDataType.Type, arrayDataType);
        }
    }
}