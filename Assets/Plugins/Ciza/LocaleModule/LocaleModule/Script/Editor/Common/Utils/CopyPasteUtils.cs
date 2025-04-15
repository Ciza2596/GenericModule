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

		#region By Type

		[NonSerialized]
		private static readonly Dictionary<Type, object> SOURCE_MAP_BY_TYPE = new Dictionary<Type, object>();

		public static void Copy(object source) =>
			Copy(source.GetType(), source);


		public static void Copy(Type type, object source)
		{
			SOURCE_MAP_BY_TYPE.Remove(type);
			SOURCE_MAP_BY_TYPE.Add(type, Duplicate(source));
		}

		public static bool CheckCanPaste(Type type) =>
			SOURCE_MAP_BY_TYPE.ContainsKey(type);

		public static bool TryPaste(Type type, out object copy)
		{
			if (!SOURCE_MAP_BY_TYPE.TryGetValue(type, out var source))
			{
				copy = null;
				return false;
			}

			copy = Duplicate(source);
			return copy != null;
		}

		#endregion

		public static object Duplicate(object source)
		{
			var sourceType = source.GetType();
			var newObj = TypeUtils.CreateInstance(sourceType);

			if (!TypeUtils.CheckIsClassWithoutString(sourceType))
				newObj = source;

			else if (sourceType.IsArray && source is object[] sourceArray)
			{
				var list = new List<object>();
				foreach (var sourceElement in sourceArray)
					list.Add(Duplicate(sourceElement));

				return list.Count > 0 ? list.ToArray() : null;
			}
			else if (source is IList sourceList)
			{
				var list = new List<object>();
				foreach (var sourceElement in sourceList)
					list.Add(Duplicate(sourceElement));

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

			if (TypeUtils.CheckIsClassWithoutString(source.GetType()))
			{
				var jsonSource = EditorJsonUtility.ToJson(source);

				var newInstance = TypeUtils.CreateInstance(source.GetType());
				EditorJsonUtility.FromJsonOverwrite(jsonSource, newInstance);
				property.SetValue(newInstance);
			}
			else
				property.SetValue(source);
		}
	}
}