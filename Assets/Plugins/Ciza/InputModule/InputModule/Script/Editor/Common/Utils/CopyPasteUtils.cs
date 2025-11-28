using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace CizaInputModule.Editor
{
	public static class CopyPasteUtils
	{
		// CONST & STATIC: -----------------------------------------------------------------------

		#region Copy

		[NonSerialized]
		private static readonly Dictionary<Type, object> SOURCE_MAP_BY_TYPE = new Dictionary<Type, object>();

		public static void Copy(object source)
		{
			if (source == null)
				return;

			Copy(source.GetType(), source);
		}

		public static void Copy(Type sourceType, object source)
		{
			foreach (var childType in TypeUtils.GetBaseAndSelfTypes(sourceType))
				SimpleCopy(childType, sourceType, source);
		}

		public static void SimpleCopy(Type itemType, Type sourceType, object source)
		{
			SOURCE_MAP_BY_TYPE.Remove(itemType);
			var copy = Duplicate(sourceType, source);
			if (copy == null)
				return;
			SOURCE_MAP_BY_TYPE.Add(itemType, copy);
		}

		public static bool CheckCanPaste(Type itemType) =>
			SOURCE_MAP_BY_TYPE.ContainsKey(itemType);

		public static bool TryPaste(Type itemType, out object copy)
		{
			if (!SOURCE_MAP_BY_TYPE.TryGetValue(itemType, out var source))
			{
				copy = null;
				return false;
			}

			copy = Duplicate(source.GetType(), source);
			return copy != null;
		}

		#endregion

		public static object Duplicate(Type sourceType, object source)
		{
			if (!TypeUtils.CheckIsClassWithoutStringOrUnityObjSubclass(sourceType))
				return source;

			if (source is IList sourceList)
			{
				var sourceListType = sourceList.GetType();
				var list = TypeUtils.TryCreateInstance(sourceListType, out var listInstance, sourceList.Count) ? listInstance as IList : null;
				for (var i = 0; i < sourceList.Count; i++)
				{
					var sourceElement = i < sourceList.Count ? sourceList[i] : null;
					object element;
					if (sourceElement != null)
						element = Duplicate(sourceElement.GetType(), sourceElement);

					else
					{
						var elementType = TypeUtils.GetElementTypes(sourceListType)[0];
						element = TypeUtils.TryCreateInstance(elementType, out var localInstance) ? localInstance : null;
					}

					if (sourceType.IsArray)
						list[i] = element;
					else
						list.Add(element);
				}

				return list;
			}

			var newObj = TypeUtils.TryCreateInstance(sourceType, out var instance) ? instance : null;
			OverrideObj(source, newObj);
			return newObj;
		}

		public static void OverrideObj(object source, object newObj)
		{
			var json = EditorJsonUtility.ToJson(source);
			EditorJsonUtility.FromJsonOverwrite(json, newObj);
		}
	}
}