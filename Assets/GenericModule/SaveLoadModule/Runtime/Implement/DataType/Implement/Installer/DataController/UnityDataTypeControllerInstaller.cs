using System;
using System.Collections.Generic;
using System.Linq;


namespace DataType.Implement
{
    public class UnityDataTypeControllerInstaller : IDataTypeControllerInstaller
    {
        private Dictionary<Type, DataType> _dataTypes;

        public void Install(Dictionary<Type, DataType> dataTypes, IDataTypeController dataTypeController,
            IReflectionHelper reflectionHelper)
        {
            _dataTypes = dataTypes;

            var floatDataType = _dataTypes.Values.First(dataType => dataType is FloatDataType) as FloatDataType;
            var intDataType = _dataTypes.Values.First(dataType => dataType is IntDataType) as IntDataType;


            var vector2DataType = new Vector2DataType(floatDataType, dataTypeController, reflectionHelper);
            AddDataTypeToDataTypes(vector2DataType,
                new Vector2ArrayDataType(vector2DataType, dataTypeController, reflectionHelper));

            var vector2IntDataType = new Vector2IntDataType(intDataType, dataTypeController, reflectionHelper);
            AddDataTypeToDataTypes(vector2IntDataType,
                new Vector2IntArrayDataType(vector2IntDataType, dataTypeController, reflectionHelper));

            var vector3DataType = new Vector3DataType(floatDataType, dataTypeController, reflectionHelper);
            AddDataTypeToDataTypes(vector3DataType,
                new Vector3ArrayDataType(vector3DataType, dataTypeController, reflectionHelper));

            var vector3IntDataType = new Vector3IntDataType(intDataType, dataTypeController, reflectionHelper);
            AddDataTypeToDataTypes(vector3IntDataType,
                new Vector3IntArrayDataType(vector3IntDataType, dataTypeController, reflectionHelper));


            _dataTypes = null;
        }

        private void AddDataTypeToDataTypes(DataType dataType, ArrayDataType arrayDataType)
        {
            _dataTypes.Add(dataType.Type, dataType);
            _dataTypes.Add(arrayDataType.Type, arrayDataType);
        }
    }
}