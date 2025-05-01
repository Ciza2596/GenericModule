using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace CizaLocaleModule.Editor
{
	public static class TypeUtils
	{
		// PUBLIC METHOD: ----------------------------------------------------------------------

		#region Check

		public static bool CheckIsUnityObj(Type type) =>
			type.IsSubclassOf(typeof(Object));

		public static bool CheckIsString(Type type) =>
			type == typeof(string);

		public static bool CheckIsClassWithoutString(Type type) =>
			type.IsClass && !CheckIsString(type);

		#endregion

		public static object CreateInstance(Type type, params object[] args)
		{
			if (CheckIsUnityObj(type))
				return null;

			if (CheckIsString(type))
				return string.Empty;

			if (type.IsArray)
				return Array.CreateInstance(type.GetElementType(), 0);

			if (!type.IsValueType && (type.IsAbstract || type.IsInterface || type.GetConstructor(Type.EmptyTypes) == null))
				throw new InvalidOperationException($"Type {type.Name} cant created by activator,");
			return Activator.CreateInstance(type, args);
		}

		#region BaseTypes

		public static Type[] GetSelfAndBaseTypes(Type type) =>
			GetBaseAndSelfTypes(type, true);

		private static Type[] GetBaseAndSelfTypes(Type type, bool isReverse)
		{
			var types = new List<Type>();
			types.AddRange(GetBaseTypes(type));
			types.Add(type);
			if (isReverse)
				types.Reverse();
			return types.ToArray();
		}

		public static Type[] GetBaseTypes(Type type)
		{
			var types = new List<Type>();
			var baseType = type.BaseType;
			while (baseType != null)
			{
				types.Add(baseType);
				baseType = baseType.BaseType;
			}

			types.Reverse();
			return types.ToArray();
		}

		#endregion
	}
}