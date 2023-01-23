using System;
using System.Collections;
using System.Collections.Generic;

namespace DataType
{
	[UnityEngine.Scripting.Preserve]
	public static class OldDataTypeManager
	{
        private static object _lock = new object();

		[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
		public static Dictionary<Type, DataType> _typeDatas = null;

        // We cache the last accessed type as we quite often use the same type multiple times,
        // so this improves performance as another lookup is not required.
        private static DataType _lastAccessedType = null;

		public static DataType GetOrCreateDataType(Type type, bool throwException = true)
		{
			if(_typeDatas == null)
				Init();

            if (type != typeof(object) && _lastAccessedType != null && _lastAccessedType.Type == type)
                return _lastAccessedType;

			// If type doesn't exist, create one.
			if(_typeDatas.TryGetValue(type, out _lastAccessedType))
				return _lastAccessedType;
			return (_lastAccessedType = CreateDataType(type, throwException));
		}

		public static DataType GetDataType(Type type)
		{
			if(_typeDatas == null)
				Init();

			if(_typeDatas.TryGetValue(type, out _lastAccessedType))
				return _lastAccessedType;
			return null;
		}

		internal static void Add(Type type, DataType es3Type)
		{
			if(_typeDatas == null)
				Init();

            var existingType = GetDataType(type);
            // if (existingType != null && existingType.Priority > es3Type.Priority)
            //     return;

            lock (_lock)
            {
                _typeDatas[type] = es3Type;
            }
		}

		internal static DataType CreateDataType(Type type, bool throwException = true)
		{
			DataType dataType = null;

			if(OldReflectionHelper.IsEnum(type))
				return new EnumDataType(type);
			else if(OldReflectionHelper.TypeIsArray(type))
			{
				int rank = OldReflectionHelper.GetArrayRank(type);
				if(rank == 1)
					dataType = new ArrayDataType(type, null);
				else if(rank == 2)
					dataType = new Array2DDataType(type, null);
				else if(rank == 3)
					dataType = new Array3DDataType(type, null);
				else if(throwException)
					throw new NotSupportedException("Only arrays with up to three dimensions are supported by Easy Save.");
				else
					return null;
			}
			else if(OldReflectionHelper.IsGenericType(type) && OldReflectionHelper.ImplementsInterface(type, typeof(IEnumerable)))
			{
				Type genericType = OldReflectionHelper.GetGenericTypeDefinition(type);
                if (typeof(List<>).IsAssignableFrom(genericType))
                    dataType = new ListDataType(type, null);
                else if (typeof(IDictionary).IsAssignableFrom(genericType))
                    dataType = new DictionaryDataType(type);
                else if (genericType == typeof(Queue<>))
                    dataType = new QueueDataType(type, null);
                else if (genericType == typeof(Stack<>))
                    dataType = new StackDataType(type, null);
                else if (genericType == typeof(HashSet<>))
                    dataType = new HashSetDataType(type);
                // else if (genericType == typeof(Unity.Collections.NativeArray<>))
                //     dataType = new ES3NativeArrayType(type);
                else if (throwException)
                    throw new NotSupportedException("Generic type \"" + type.ToString() + "\" is not supported by Easy Save.");
                else
                    return null;
			}
			else if(OldReflectionHelper.IsPrimitive(type)) // ERROR: We should not have to create an ES3Type for a primitive.
			{
				if(_typeDatas == null || _typeDatas.Count == 0)	// If the type list is not initialised, it is most likely an initialisation error.
					throw new TypeLoadException("ES3Type for primitive could not be found, and the type list is empty. Please contact Easy Save developers at http://www.moodkie.com/contact");
				else // Else it's a different error, possibly an error in the specific ES3Type for that type.
					throw new TypeLoadException("ES3Type for primitive could not be found, but the type list has been initialised and is not empty. Please contact Easy Save developers on mail@moodkie.com");
			}
			else
			{
                // if (ES3Reflection.IsAssignableFrom(typeof(Component), type))
                //     dataType = new ES3ReflectedComponentType(type);
                // else if (ES3Reflection.IsValueType(type))
                //     dataType = new ES3ReflectedValueType(type);
                // else if (ES3Reflection.IsAssignableFrom(typeof(ScriptableObject), type))
                //     dataType = new ES3ReflectedScriptableObjectType(type);
                // else if (ES3Reflection.IsAssignableFrom(typeof(UnityEngine.Object), type))
                //     dataType = new ES3ReflectedUnityObjectType(type);
                // /*else if (ES3Reflection.HasParameterlessConstructor(type) || ES3Reflection.IsAbstract(type) || ES3Reflection.IsInterface(type))
                //     es3Type = new ES3ReflectedObjectType(type);*/
                // else if (type.Name.StartsWith("Tuple`"))
                //     dataType = new ES3TupleType(type);
                // /*else if (throwException)
                //     throw new NotSupportedException("Type of " + type + " is not supported as it does not have a parameterless constructor. Only value types, Components or ScriptableObjects are supportable without a parameterless constructor. However, you may be able to create an ES3Type script to add support for it.");*/
                // else
                //     dataType = new ES3ReflectedObjectType(type);
            }

			// if(dataType.Type == null || dataType.isUnsupported)
			// {
			// 	if(throwException)
			// 		throw new NotSupportedException(string.Format("ES3Type.type is null when trying to create an ES3Type for {0}, possibly because the element type is not supported.", type));
			// 	return null;
			// }

            Add(type, dataType);
			return dataType;
		}

        internal static void Init()
        {
            lock (_lock)
            {
                _typeDatas = new Dictionary<Type, DataType>();
                // ES3Types add themselves to the types Dictionary.
                OldReflectionHelper.GetInstances<DataType>();

                // Check that the type list was initialised correctly.
                if (_typeDatas == null || _typeDatas.Count == 0)
                    throw new TypeLoadException("Type list could not be initialised. Please contact Easy Save developers on mail@moodkie.com.");
            }
        }
	}
}
