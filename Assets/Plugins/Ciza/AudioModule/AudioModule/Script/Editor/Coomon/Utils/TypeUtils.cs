using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace CizaAudioModule.Editor
{
	public static class TypeUtils
	{
		private static readonly char[] ASSEMBLY_SEPARATOR = { ' ' };

		// PUBLIC METHOD: ----------------------------------------------------------------------

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
			var types = GetSelfAndBaseTypes(property.GetValue().GetType());
			var allGenericTypes = new List<Type>();
			foreach (var type in types)
			{
				var genericTypes = type.GetGenericArguments();
				if (genericTypes.Length > 0)
					allGenericTypes.AddRange(genericTypes);
			}

			return allGenericTypes.ToArray();
		}

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

		public static bool IsClass(this SerializedProperty property)
		{
			if (property.propertyType != SerializedPropertyType.Generic) return false;

			var copy = property.Copy();
			return copy.Next(true);
		}
	}
}