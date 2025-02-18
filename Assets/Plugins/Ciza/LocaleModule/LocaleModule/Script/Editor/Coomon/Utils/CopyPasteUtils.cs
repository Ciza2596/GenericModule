using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace CizaLocaleModule.Editor
{
	public static class CopyPasteUtils
	{
		// CONST & STATIC: -----------------------------------------------------------------------

		[NonSerialized]
		private static readonly Dictionary<Type, object> SOURCE_MAP_LIST_BY_TYPE = new Dictionary<Type, object>();

		public static void Copy(object source)
		{
			var type = source.GetType();
			SOURCE_MAP_LIST_BY_TYPE.Remove(type);
			SOURCE_MAP_LIST_BY_TYPE.Add(type, Duplicate(source.GetType(), source));
			OnCopy?.Invoke();
		}

		public static bool CheckCanPaste(Type type) =>
			SOURCE_MAP_LIST_BY_TYPE.ContainsKey(type);

		public static bool TryPaste(Type type, out object copy)
		{
			if (!SOURCE_MAP_LIST_BY_TYPE.TryGetValue(type, out var source))
			{
				copy = null;
				return false;
			}

			copy = Duplicate(source.GetType(), source);
			OnPaste?.Invoke();
			return copy != null;
		}

		public static object Duplicate(Type sourceType, object source)
		{
			var newObj = TypeUtils.CreateInstance(sourceType);

			if (!sourceType.CheckIsClassWithoutString())
				newObj = source;

			else if (sourceType.IsArray && source is object[] sourceArray)
			{
				var list = new List<object>();
				foreach (var sourceElement in sourceArray)
					list.Add(Duplicate(sourceElement.GetType(), sourceElement));

				return list.Count > 0 ? list.ToArray() : null;
			}
			else if (source is IList sourceList)
			{
				var list = new List<object>();
				foreach (var sourceElement in sourceList)
					list.Add(Duplicate(sourceElement.GetType(), sourceElement));

				return list.Count > 0 ? list.ToList() : null;
			}
			else
				OverrideObj(source, newObj);

			return newObj;
		}

		public static void OverrideObj(object source, object newObj)
		{
			var json = EditorJsonUtility.ToJson(source);
			EditorJsonUtility.FromJsonOverwrite(json, newObj);
		}

		public static void Duplicate(SerializedProperty property, object source)
		{
			if (source == null)
				return;

			if (source.GetType().CheckIsClassWithoutString())
			{
				var jsonSource = EditorJsonUtility.ToJson(source);

				var newInstance = TypeUtils.CreateInstance(source.GetType());
				EditorJsonUtility.FromJsonOverwrite(jsonSource, newInstance);
				property.SetValue(newInstance);
			}
			else
				property.SetValue(source);
		}


		// EVENT: ---------------------------------------------------------------------------------

		public static event Action OnCopy;
		public static event Action OnPaste;
	}
}