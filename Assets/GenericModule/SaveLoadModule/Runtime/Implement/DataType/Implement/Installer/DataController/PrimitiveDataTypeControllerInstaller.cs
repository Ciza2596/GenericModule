using System;
using System.Collections.Generic;


namespace DataType.Implement
{
    public class PrimitiveDataTypeControllerInstaller : IDataTypeControllerInstaller
    {
        private Dictionary<Type, DataType> _dataTypes;

        public void Install(Dictionary<Type, DataType> dataTypes, IDataTypeController dataTypeController,
            IReflectionHelper reflectionHelper)
        {
            _dataTypes = dataTypes;

            var boolDataType = new BoolDataType(dataTypeController, reflectionHelper);
            AddDataTypeToDataTypes(boolDataType,
                new BoolArrayDataType(boolDataType, dataTypeController, reflectionHelper));

            var charDataType = new CharDataType(dataTypeController, reflectionHelper);
            AddDataTypeToDataTypes(charDataType,
                new CharArrayDataType(charDataType, dataTypeController, reflectionHelper));

            var longDataType = new LongDataType(dataTypeController, reflectionHelper);
            AddDataTypeToDataTypes(longDataType,
                new LongArrayDataType(longDataType, dataTypeController, reflectionHelper));

            var dateTimeDataType = new DateTimeDataType(longDataType, dataTypeController, reflectionHelper);
            AddDataTypeToDataTypes(dateTimeDataType,
                new DateTimeArrayDataType(dateTimeDataType, dataTypeController, reflectionHelper));

            var doubleDataType = new DoubleDataType(dataTypeController, reflectionHelper);
            AddDataTypeToDataTypes(doubleDataType,
                new DoubleArrayDataType(doubleDataType, dataTypeController, reflectionHelper));

            var floatDataType = new FloatDataType(dataTypeController, reflectionHelper);
            AddDataTypeToDataTypes(floatDataType,
                new FloatArrayDataType(floatDataType, dataTypeController, reflectionHelper));

            var intDataType = new IntDataType(dataTypeController, reflectionHelper);
            AddDataTypeToDataTypes(intDataType,
                new IntArrayDataType(intDataType, dataTypeController, reflectionHelper));

            var shortDataType = new ShortDataType(dataTypeController, reflectionHelper);
            AddDataTypeToDataTypes(shortDataType,
                new ShortArrayDataType(shortDataType, dataTypeController, reflectionHelper));

            var stringDataType = new StringDataType(dataTypeController, reflectionHelper);
            AddDataTypeToDataTypes(stringDataType,
                new StringArrayDataType(stringDataType, dataTypeController, reflectionHelper));

            _dataTypes = null;
        }


        private void AddDataTypeToDataTypes(DataType dataType, ArrayDataType arrayDataType)
        {
            _dataTypes.Add(dataType.Type, dataType);
            _dataTypes.Add(arrayDataType.Type, arrayDataType);
        }
    }
}