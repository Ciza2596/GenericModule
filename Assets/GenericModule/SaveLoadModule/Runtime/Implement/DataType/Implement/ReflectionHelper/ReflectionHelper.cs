using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace DataType.Implement
{
	[Serializable]
	public class ReflectionHelper : IReflectionHelper
	{
		//private variable
		private readonly Type[] _serializableAttributeTypes;
		private readonly Type[] _nonSerializableAttributeTypes;

		//private IDataTypeController _dataTypeController;

		public ReflectionHelper(IReflectionHelperConfig reflectionHelperConfig)
		{
			_serializableAttributeTypes    = m_CreateAttributeTypes(new[] { typeof(SerializeField) }, reflectionHelperConfig.CustomSerializableAttributeTypes);
			_nonSerializableAttributeTypes = m_CreateAttributeTypes(new[] { typeof(NonSerializedAttribute) }, reflectionHelperConfig.CustomNonSerializableAttributeTypes);

			Type[] m_CreateAttributeTypes(Type[] m_defaultAttributeTypes, Type[] m_customAttributeTypes)
			{
				var m_attributeTypes = m_defaultAttributeTypes.ToList();
				m_attributeTypes.AddRange(m_customAttributeTypes);

				return m_attributeTypes.ToArray();
			}
		}

		// public void Initialize(IDataTypeController dataTypeController) =>
		// 	_dataTypeController = dataTypeController;

		public System.Object CreateInstance(Type type) =>
			Activator.CreateInstance(type);

		public System.Object CreateInstance(Type type, params object[] args) =>
			Activator.CreateInstance(type, args);

		public Array CreateArrayInstance(Type type, int length) =>
			Array.CreateInstance(type, new[] { length });

		public Array CreateArrayInstance(Type type, int[] dimensions) =>
			Array.CreateInstance(type, dimensions);

		public Type MakeGenericType(Type type, Type genericArgs) =>
			type.MakeGenericType(genericArgs);

		public MethodInfo[] GetMethods(Type type, string methodName) =>
			type.GetMethods().Where(t => t.Name == methodName).ToArray();

		public bool CheckIsEnum(Type type) =>
			type.IsEnum;

		public bool CheckIsImplementsInterface(Type type, Type interfaceType) =>
			type.GetInterface(interfaceType.Name) != null;

		public bool CheckIsArray(Type type) =>
			type.IsArray;

		public bool CheckIsGenericType(Type type) =>
			type.IsGenericType;

		public bool CheckIsAssignableFrom(Type a, Type b) =>
			a.IsAssignableFrom(b);

		public int GetArrayRank(Type type) =>
			type.GetArrayRank();

		public Type GetGenericTypeDefinition(Type type) =>
			type.GetGenericTypeDefinition();

		public string GetTypeName(Type type)
		{
			if (type == typeof(bool))
				return "bool";
			if (type == typeof(byte))
				return "byte";
			if (type == typeof(sbyte))
				return "sbyte";
			if (type == typeof(char))
				return "char";
			if (type == typeof(decimal))
				return "decimal";
			if (type == typeof(double))
				return "double";
			if (type == typeof(float))
				return "float";
			if (type == typeof(int))
				return "int";
			if (type == typeof(uint))
				return "uint";
			if (type == typeof(long))
				return "long";
			if (type == typeof(ulong))
				return "ulong";
			if (type == typeof(short))
				return "short";
			if (type == typeof(ushort))
				return "ushort";
			if (type == typeof(string))
				return "string";
			if (type == typeof(Vector2))
				return "Vector2";
			if (type == typeof(Vector3))
				return "Vector3";

			return GetAssemblyTypeName(type);
		}

		public Type GetType(string typeString)
		{
			switch (typeString)
			{
				case "bool":
					return typeof(bool);
				case "byte":
					return typeof(byte);
				case "sbyte":
					return typeof(sbyte);
				case "char":
					return typeof(char);
				case "decimal":
					return typeof(decimal);
				case "double":
					return typeof(double);
				case "float":
					return typeof(float);
				case "int":
					return typeof(int);
				case "uint":
					return typeof(uint);
				case "long":
					return typeof(long);
				case "ulong":
					return typeof(ulong);
				case "short":
					return typeof(short);
				case "ushort":
					return typeof(ushort);
				case "string":
					return typeof(string);
				case "Vector2":
					return typeof(Vector2);
				case "Vector3":
					return typeof(Vector3);
				default:
					return Type.GetType(typeString);
			}
		}

		public Type[] GetElementTypes(Type type)
		{
			if (CheckIsGenericType(type))
				return GetGenericArguments(type);

			if (type.IsArray)
				return new[] { GetElementType(type) };

			return null;
		}

		public Type[] GetGenericArguments(Type type) =>
			type.GetGenericArguments();

		public Type GetBaseType(Type type) =>
			type.BaseType;

		public IProperty[] AddSerializableProperties(Type type, string[] propertyNames)
		{
			if (type is null)
				return Array.Empty<IProperty>();

			var properties = new List<IProperty>();
			AddSerializableFields(type, properties, propertyNames);
			AddSerializableProperties(type, properties);
			return properties.ToArray();
		}

		//private method

		private bool CheckIsValueType(Type type) =>
			type.IsValueType;

		private bool CheckIsPrimitive(Type type) =>
			type.IsPrimitive || type == typeof(string) || type == typeof(decimal);

		private bool CheckIsSerializable(Type type)
		{
			if (type is null)
				return false;

			if (CheckIsPrimitive(type) || CheckIsValueType(type))
				return true;

			if (CheckIsDefined(type, typeof(SerializableAttribute)))
				return true;


			// var dataType = _dataTypeController.GetOrCreateDataType(type);
			// if (dataType != null && !dataType.IsUnsupported)
			// 	return true;
			//
			// if (CheckIsArray(type))
			// {
			// 	if (CheckIsSerializable(type.GetElementType()))
			// 		return true;
			//
			// 	return false;
			// }

			// var genericArgs = type.GetGenericArguments();
			// foreach (var genericArg in genericArgs)
			// 	if (!CheckIsSerializable(genericArg))
			// 		return false;

			return false;
		}

		private bool CheckIsDefined(MemberInfo info, Type[] attributeTypes)
		{
			foreach (var attributeType in attributeTypes)
				if (CheckIsDefined(info, attributeType))
					return true;

			return false;
		}

		private bool CheckIsDefined(MemberInfo info, Type attributeType) =>
			Attribute.IsDefined(info, attributeType, true);

		private bool CheckIsDefined(Type type, Type attributeType) =>
			Attribute.IsDefined(type, attributeType, true);

		private Type GetElementType(Type type) =>
			type.GetElementType();

		private void AddSerializableFields(Type type, List<IProperty> serializableFields, string[] fieldNames = null, BindingFlags bindings = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly)
		{
			var fieldInfos = type.GetFields(bindings);
			foreach (var fieldInfo in fieldInfos)
			{
				// Exclude const or readonly fields.
				if (fieldInfo.IsLiteral || fieldInfo.IsInitOnly)
					continue;

				// If a members array was provided as a parameter, only include the field if it's in the array.
				if (fieldNames != null && !fieldNames.Contains(fieldInfo.Name))
					continue;

				if (CheckIsDefined(fieldInfo, _nonSerializableAttributeTypes))
					continue;

				if (!CheckIsDefined(fieldInfo, _serializableAttributeTypes))
					continue;

				if (!CheckIsSerializable(type))
					continue;

				var field = new Field(fieldInfo);
				serializableFields.Add(field);
			}
		}

		private void AddSerializableProperties(Type type, List<IProperty> serializableProperties, BindingFlags bindings = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly)
		{
			var propertyInfos = type.GetProperties(bindings);
			foreach (var propertyInfo in propertyInfos)
			{
				if (!propertyInfo.CanWrite)
					continue;

				if (CheckIsDefined(propertyInfo, _nonSerializableAttributeTypes))
					continue;

				if (!CheckIsDefined(propertyInfo, _serializableAttributeTypes))
					continue;

				if (!CheckIsSerializable(type))
					continue;

				var property = new Property(propertyInfo);
				serializableProperties.Add(property);
			}

			var baseType = GetBaseType(type);
			if (baseType != null && baseType != typeof(System.Object))
				AddSerializableProperties(baseType, serializableProperties);
		}

		private string GetAssemblyTypeName(Type type)
		{
			if (CheckIsPrimitive(type))
				return type.ToString();
			return type.FullName + "," + type.Assembly.GetName().Name;
		}
	}
}
