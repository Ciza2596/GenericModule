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
		private static readonly char[] SEPARATOR = { '.' };

		// PUBLIC METHOD: ----------------------------------------------------------------------

		public static string GetNiceName(Type type) => GetNiceName(type.ToString());

		public static string GetNiceName(string type)
		{
			var split = type.Split(SEPARATOR);
			return split.Length > 0 ? TextUtils.Humanize(split[^1]) : string.Empty;
		}


		public static string GetTitle(SerializedProperty property, bool isFullType, HashSet<string> forbiddenNames = null) => GetTitle(GetType(property, isFullType), forbiddenNames);

		public static string GetTitle(Type type, HashSet<string> forbiddenNames = null)
		{
			if (type == null)
				return "(none)";
			var title = type.GetCustomAttributes<TitleAttribute>().FirstOrDefault();

			var titleName = title != null && !string.IsNullOrEmpty(title.Title) ? title.Title : GetNiceName(type);

			if (forbiddenNames == null) return titleName;
			if (string.IsNullOrEmpty(titleName)) return titleName;

			var number = 1;
			var complete = titleName;

			while (forbiddenNames.Contains(complete))
			{
				complete = $"{titleName} ({number})";
				number += 1;
			}

			return complete;
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

		// PRIVATE METHOD: --------------------------------------------------------------------- 

		private static Type[] GetDerivedTypes(Type type)
		{
			var assemblies = AppDomain.CurrentDomain.GetAssemblies();
			var types = new List<Type>();

			if (type == null) return types.ToArray();

			foreach (var assembly in assemblies)
			{
				var assemblyTypes = assembly.GetTypes();
				foreach (var assemblyType in assemblyTypes)
				{
					if (assemblyType.IsInterface) continue;
					if (assemblyType.IsAbstract) continue;
					if (type.IsAssignableFrom(assemblyType)) types.Add(assemblyType);
				}
			}

			return types.ToArray();
		}
	}
}