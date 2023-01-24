using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace DataType
{
    public class DataTypeController
    {
        //private variable
        private readonly Dictionary<Type, DataType> _dataTypes = new Dictionary<Type, DataType>();
        private readonly IReflectionHelper _reflectionHelper;


        //public method
        public DataTypeController(IReflectionHelper reflectionHelper, IDataTypeInstaller[] dataTypeInstallers)
        {
            _reflectionHelper = reflectionHelper;
            foreach (var dataTypeInstaller in dataTypeInstallers)
                dataTypeInstaller.Install(_dataTypes, _reflectionHelper);
        }


        public DataType GetOrCreateDataType(Type key)
        {
            if (_dataTypes.TryGetValue(key, out var dataType))
                return dataType;

            return CreateDataType(key, _reflectionHelper);
        }


        //private method

        private DataType CreateDataType(Type type, IReflectionHelper reflectionHelper)
        {
            if (reflectionHelper.CheckIsEnum(type))
                return new EnumDataType(type);

            DataType dataType = null;

            if (reflectionHelper.CheckIsArray(type))
            {
                var rank = reflectionHelper.GetArrayRank(type);
                switch (rank)
                {
                    case 1:
                        dataType = new ArrayDataType(type, reflectionHelper);
                        break;

                    case 2:
                        dataType = new Array2DDataType(type, reflectionHelper);
                        break;

                    case 3:
                        dataType = new Array3DDataType(type, reflectionHelper);
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
                
                if (typeof(List<>).IsAssignableFrom(genericType))
                    dataType = new ListDataType(type, reflectionHelper);
                
                else if (typeof(IDictionary).IsAssignableFrom(genericType))
                    dataType = new DictionaryDataType(type);
                
                else if (genericType == typeof(Queue<>))
                    dataType = new QueueDataType(type, reflectionHelper);
                
                else if (genericType == typeof(Stack<>))
                    dataType = new StackDataType(type, reflectionHelper);
                
                else if (genericType == typeof(HashSet<>))
                    dataType = new HashSetDataType(type);
                
                else
                    Debug.LogError(
                        $"[DataTypeController::CreateDataType] Type: {type} is IEnumerable and unsupported for save.");
            }

            Assert.IsNotNull(dataType, $"[DataTypeController::CreateDataType] Type: {type} is unsupported for save.");

            if (!_dataTypes.ContainsKey(type))
                _dataTypes.Add(type, dataType);

            return dataType;
        }
    }
}