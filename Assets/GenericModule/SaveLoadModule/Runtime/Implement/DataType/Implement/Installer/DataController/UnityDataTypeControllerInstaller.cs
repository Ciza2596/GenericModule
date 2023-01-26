using System;
using System.Collections.Generic;
using System.Linq;


namespace DataType.Implement
{
    public class UnityDataTypeControllerInstaller : IDataTypeControllerInstaller
    {
        private Dictionary<Type, DataType> _dataTypes;

        public void Install(Dictionary<Type, DataType> dataTypes, IReflectionHelper reflectionHelper)
        {
            _dataTypes = dataTypes;

            var floatDataType = _dataTypes.Values.First(dataType => dataType is FloatDataType) as FloatDataType;
            var intDataType = _dataTypes.Values.First(dataType => dataType is IntDataType) as IntDataType;


            var vector2DataType = new Vector2DataType(floatDataType);
            AddDataTypeToDataTypes(vector2DataType, new Vector2ArrayDataType(vector2DataType, reflectionHelper));

            var vector2IntDataType = new Vector2IntDataType(intDataType);
            AddDataTypeToDataTypes(vector2IntDataType,
                new Vector2IntArrayDataType(vector2IntDataType, reflectionHelper));

            var vector3DataType = new Vector3DataType(floatDataType);
            AddDataTypeToDataTypes(vector3DataType, new Vector3ArrayDataType(vector3DataType, reflectionHelper));

            var vector3IntDataType = new Vector3IntDataType(intDataType);
            AddDataTypeToDataTypes(vector3IntDataType,
                new Vector3IntArrayDataType(vector3IntDataType, reflectionHelper));


            _dataTypes = null;
        }

        private void AddDataTypeToDataTypes(DataType dataType, ArrayDataType arrayDataType)
        {
            _dataTypes.Add(dataType.Type, dataType);
            _dataTypes.Add(arrayDataType.Type, arrayDataType);
        }
    }
}