using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CizaSaveLoadModule.Implement;
using UnityEngine;
using UnityEngine.Scripting;

namespace DataType.Implement
{
	[Serializable]
	public class ReflectionHelper : IReflectionHelper
	{
		//private variable
		private readonly Type[] _serializableAttributeTypes;
		private readonly Type[] _nonSerializableAttributeTypes;

		[Preserve]
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
			if (type == TagUtils.BoolType)
				return TagUtils.BoolType.Name;

			if (type == TagUtils.ByteType)
				return TagUtils.ByteType.Name;

			if (type == TagUtils.SbyteType)
				return TagUtils.SbyteType.Name;

			if (type == TagUtils.CharType)
				return TagUtils.CharType.Name;

			if (type == TagUtils.DecimalType)
				return TagUtils.DecimalType.Name;

			if (type == TagUtils.DoubleType)
				return TagUtils.DoubleType.Name;

			if (type == TagUtils.FloatType)
				return TagUtils.FloatType.Name;

			if (type == TagUtils.IntType)
				return TagUtils.IntType.Name;

			if (type == TagUtils.UintType)
				return TagUtils.UintType.Name;

			if (type == TagUtils.LongType)
				return TagUtils.LongType.Name;

			if (type == TagUtils.UlongType)
				return TagUtils.UlongType.Name;

			if (type == TagUtils.ShortType)
				return TagUtils.ShortType.Name;

			if (type == TagUtils.UshortType)
				return TagUtils.UshortType.Name;

			if (type == TagUtils.StringType)
				return TagUtils.StringType.Name;

			if (type == TagUtils.Vector2Type)
				return TagUtils.Vector2Type.Name;

			if (type == TagUtils.Vector3Type)
				return TagUtils.Vector3Type.Name;

			return GetAssemblyTypeName(type);
		}

		public Type GetType(string typeString)
		{
			if (typeString == TagUtils.BoolType.Name)
				return TagUtils.BoolType;
			if (typeString == TagUtils.ByteType.Name)
				return TagUtils.ByteType;
			if (typeString == TagUtils.SbyteType.Name)
				return TagUtils.SbyteType;
			if (typeString == TagUtils.CharType.Name)
				return TagUtils.CharType;
			if (typeString == TagUtils.DecimalType.Name)
				return TagUtils.DecimalType;
			if (typeString == TagUtils.DoubleType.Name)
				return TagUtils.DoubleType;
			if (typeString == TagUtils.FloatType.Name)
				return TagUtils.FloatType;
			if (typeString == TagUtils.IntType.Name)
				return TagUtils.IntType;
			if (typeString == TagUtils.UintType.Name)
				return TagUtils.UintType;
			if (typeString == TagUtils.LongType.Name)
				return TagUtils.LongType;
			if (typeString == TagUtils.UlongType.Name)
				return TagUtils.UlongType;
			if (typeString == TagUtils.ShortType.Name)
				return TagUtils.ShortType;
			if (typeString == TagUtils.UshortType.Name)
				return TagUtils.UshortType;
			if (typeString == TagUtils.StringType.Name)
				return TagUtils.StringType;
			if (typeString == TagUtils.Vector2Type.Name)
				return TagUtils.Vector2Type;
			if (typeString == TagUtils.Vector3Type.Name)
				return TagUtils.Vector3Type;

			return Type.GetType(typeString);
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
			return type.FullName + TagUtils.COMMA_TAG + type.Assembly.GetName().Name;
		}
	}
}
