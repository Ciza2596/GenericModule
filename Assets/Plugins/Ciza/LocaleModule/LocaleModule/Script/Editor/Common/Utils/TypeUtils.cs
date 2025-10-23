using System;
using System.Collections;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace CizaLocaleModule.Editor
{
	public static class TypeUtils
	{
		// PUBLIC METHOD: ----------------------------------------------------------------------

		#region Check

		public static bool CheckIsListImp(Type type) =>
			typeof(IList).IsAssignableFrom(type);

		public static bool CheckIsUnityObjSubclass(Type type) =>
			type.IsSubclassOf(typeof(Object));

		public static bool CheckIsString(Type type) =>
			type == typeof(string);

		public static bool CheckIsClassWithoutStringOrUnityObjSubclass(Type type) =>
			type.IsClass && !CheckIsString(type) && !CheckIsUnityObjSubclass(type);

		public static bool CheckIsAbstractOrInterface(Type type) =>
			type.IsAbstract || type.IsInterface;

		#endregion

		public static bool TryCreateInstance(Type type, out object instance, params object[] args)
		{
			if (CheckIsUnityObjSubclass(type))
			{
				instance = null;
				return true;
			}

			if (CheckIsString(type))
			{
				instance = string.Empty;
				return true;
			}

			if (type.IsArray)
			{
				instance = Array.CreateInstance(GetElementTypes(type)[0], (args.Length == 1 && args[0] is int length) ? length : 0);
				return true;
			}

			if (CheckIsListImp(type))
			{
				var listType = typeof(List<>).MakeGenericType(GetElementTypes(type)[0]);
				instance = (IList)Activator.CreateInstance(listType);
				return true;
			}

			if (!type.IsValueType && (CheckIsAbstractOrInterface(type) || type.GetConstructor(Type.EmptyTypes) == null))
			{
				instance = null;
				return false;
			}

			instance = Activator.CreateInstance(type, args);
			return true;
		}

		public static Type[] GetElementTypes(Type type)
		{
			var types = GetSelfAndBaseTypes(type);
			var allGenericTypes = new List<Type>();
			foreach (var childType in types)
			{
				if (childType.IsArray)
					allGenericTypes.Add(childType.GetElementType());
				else
				{
					var genericTypes = childType.GetGenericArguments();
					if (genericTypes.Length > 0)
						allGenericTypes.AddRange(genericTypes);
				}
			}

			return allGenericTypes.ToArray();
		}

		#region BaseTypes

		public static Type[] GetSelfAndBaseTypes(Type type) =>
			GetBaseAndSelfTypes(type, true);

		public static Type[] GetBaseAndSelfTypes(Type type) =>
			GetBaseAndSelfTypes(type, false);

		private static Type[] GetBaseAndSelfTypes(Type type, bool isReverse)
		{
			var types = new List<Type>();
			types.AddRange(GetBaseTypes(type, true));
			types.Add(type);
			if (isReverse)
				types.Reverse();
			return types.ToArray();
		}

		public static Type[] GetBaseTypes(Type type, bool isReverse = false)
		{
			var types = new List<Type>();
			var baseType = type.BaseType;
			while (baseType != null)
			{
				types.Add(baseType);
				baseType = baseType.BaseType;
			}

			if (isReverse)
				types.Reverse();
			return types.ToArray();
		}

		#endregion
	}
}