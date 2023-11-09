using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Scripting;

namespace DataType.Implement
{
    public class DataTypeController
    {
        //private variable
        private readonly Dictionary<Type, DataType> _dataTypes = new Dictionary<Type, DataType>();
        private readonly IDataTypeController _dataTypeController;
        private readonly IReflectionHelper _reflectionHelper;


        //public method
        [Preserve]
        public DataTypeController(IDataTypeController dataTypeController, IReflectionHelper reflectionHelper, IDataTypeControllerInstaller[] dataTypeInstallers)
        {
            _dataTypeController = dataTypeController;
            _reflectionHelper = reflectionHelper;

            foreach (var dataTypeInstaller in dataTypeInstallers)
                dataTypeInstaller.Install(_dataTypes, _dataTypeController, _reflectionHelper);
        }


        public DataType GetOrCreateDataType(Type key)
        {
            if (_dataTypes.TryGetValue(key, out var dataType))
                return dataType;

            return CreateDataType(key, _dataTypeController, _reflectionHelper);
        }


        //private method

        private DataType CreateDataType(Type type, IDataTypeController dataTypeController,
            IReflectionHelper reflectionHelper)
        {
            if (reflectionHelper.CheckIsEnum(type))
                return new EnumDataType(type, dataTypeController, reflectionHelper);

            DataType dataType = null;

            if (reflectionHelper.CheckIsArray(type))
            {
                var rank = reflectionHelper.GetArrayRank(type);

                var elementTypes = reflectionHelper.GetElementTypes(type);
                var elementType = elementTypes[0];
                var elementDataType = GetOrCreateDataType(elementType);

                switch (rank)
                {
                    case 1:
                        dataType = new ArrayDataType(type, elementDataType, dataTypeController, reflectionHelper);
                        break;

                    case 2:
                        dataType = new Array2DDataType(type, elementDataType, dataTypeController, reflectionHelper);
                        break;

                    case 3:
                        dataType = new Array3DDataType(type, elementDataType, dataTypeController, reflectionHelper);
                        break;

                    default:
                        Debug.LogError(
                            $"[DataTypeController::CreateDataType] ArrayRank: {rank} is unsupported for save.");
                        break;
                }
            }
            else if (reflectionHelper.CheckIsImplementsInterface(type, typeof(IEnumerable)) &&
                     reflectionHelper.CheckIsGenericType(type))
            {
                var genericType = reflectionHelper.GetGenericTypeDefinition(type);

                var elementTypes = reflectionHelper.GetElementTypes(type);

                var elementType = elementTypes[0];
                var elementDataType = GetOrCreateDataType(elementType);

                if (typeof(List<>).IsAssignableFrom(genericType))
                    dataType = new ListDataType(type, elementDataType, dataTypeController, reflectionHelper);

                else if (typeof(IDictionary).IsAssignableFrom(genericType))
                {
                    var valueElementType = elementTypes[1];
                    var valueElementDataType = GetOrCreateDataType(valueElementType);

                    dataType = new DictionaryDataType(type, elementDataType, valueElementDataType, dataTypeController,
                        reflectionHelper);
                }
                else if (genericType == typeof(Queue<>))
                    dataType = new QueueDataType(type, elementDataType, dataTypeController, reflectionHelper);

                else if (genericType == typeof(Stack<>))
                    dataType = new StackDataType(type, elementDataType, dataTypeController, reflectionHelper);

                else if (genericType == typeof(HashSet<>))
                    dataType = new HashSetDataType(type, elementDataType, dataTypeController, reflectionHelper);

                else
                    Debug.LogError(
                        $"[DataTypeController::CreateDataType] Type: {type} is IEnumerable and unsupported for save.");
            }
            else
                dataType = new ReflectedObjectDataType(type, dataTypeController, reflectionHelper);
            

            Assert.IsNotNull(dataType, $"[DataTypeController::CreateDataType] Type: {type} is unsupported for save.");

            if (!_dataTypes.ContainsKey(type))
                _dataTypes.Add(type, dataType);

            return dataType;
        }
    }
}