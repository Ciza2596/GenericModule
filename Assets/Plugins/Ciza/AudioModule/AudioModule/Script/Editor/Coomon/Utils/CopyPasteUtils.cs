using System;
using System.Collections.Generic;
using UnityEditor;

namespace CizaAudioModule.Editor
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
			SOURCE_MAP_LIST_BY_TYPE.Add(type, source);
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

			var jsonSource = EditorJsonUtility.ToJson(source);
			copy = Activator.CreateInstance(type);
			EditorJsonUtility.FromJsonOverwrite(jsonSource, copy);
			OnPaste?.Invoke();
			return true;
		}

		public static void Duplicate(SerializedProperty property, object source)
		{
			if (source == null)
				return;

			var jsonSource = EditorJsonUtility.ToJson(source);

			var newInstance = Activator.CreateInstance(source.GetType());
			EditorJsonUtility.FromJsonOverwrite(jsonSource, newInstance);

			property.SetValue(newInstance);
		}


		// EVENT: ---------------------------------------------------------------------------------

		public static event Action OnCopy;
		public static event Action OnPaste;
	}
}