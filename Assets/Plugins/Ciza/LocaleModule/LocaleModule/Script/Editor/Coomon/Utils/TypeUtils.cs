using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CizaLocaleModule.Editor
{
	public static class TypeUtils
	{
		private static readonly char[] ASSEMBLY_SEPARATOR = { ' ' };

		// PUBLIC METHOD: ----------------------------------------------------------------------

		public static object CreateInstance(Type type)
		{
			if (type.CheckIsUnityObj())
				return null;
			
			if (type == typeof(string))
				return string.Empty;

			if (type.IsArray)
				return Array.CreateInstance(type.GetElementType(), 0);

			if (!type.IsValueType && (type.IsAbstract || type.IsInterface || type.GetConstructor(Type.EmptyTypes) == null))
				throw new InvalidOperationException($"Type {type.Name} cant created by activator,");
			return Activator.CreateInstance(type);
		}

		public static Type[] GetSelfAndBaseTypes(Type type)
		{
			var types = new List<Type> { type };
			var baseType = type.BaseType;
			while (baseType != null)
			{
				types.Add(baseType);
				baseType = baseType.BaseType;
			}

			types.Reverse();
			return types.ToArray();
		}


		public static Type[] GetGenericTypes(SerializedProperty property)
		{
			var value = property.GetValue();
			var types = GetSelfAndBaseTypes(value.GetType());
			var allGenericTypes = new List<Type>();
			foreach (var type in types)
			{
				if (type.IsArray)
					allGenericTypes.Add(type.GetElementType());
				else
				{
					var genericTypes = type.GetGenericArguments();
					if (genericTypes.Length > 0)
						allGenericTypes.AddRange(genericTypes);
				}
			}

			return allGenericTypes.ToArray();
		}
		
		public static bool CheckIsUnityObj(this Type type) =>
			type.IsSubclassOf(typeof(Object));
		
		public static bool CheckIsClassWithoutString(this Type type) =>
			type.IsClass && type != typeof(string);

		public static Type GetType(SerializedProperty property, bool isFullType)
		{
			if (property == null)
			{
				Debug.LogError("Null property was found at 'GetTypeFromProperty'");
				return null;
			}

			if (property.propertyType != SerializedPropertyType.ManagedReference)
				return property.GetValue()?.GetType();

			var split = isFullType ? property.managedReferenceFullTypename.Split(ASSEMBLY_SEPARATOR) : property.managedReferenceFieldTypename.Split(ASSEMBLY_SEPARATOR);
			return split.Length != 2 ? null : Type.GetType(Assembly.CreateQualifiedName(split[0], split[1]));
		}
	}
}