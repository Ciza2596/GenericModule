using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CizaAddressablesModule.Editor
{
	public static class CopyPasteUtils
	{
		private const BindingFlags SERIALIZED_FIELD_BINDINGS = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

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

		#region Behaviour Value Copy

		[NonSerialized]
		private static readonly Dictionary<Type, object> BEHAVIOUR_VALUE_MAP_BY_TYPE = new Dictionary<Type, object>();

		public static void CopyBehaviourValue(object source) =>
			CopyBehaviourValue(source.GetType(), source);


		public static void CopyBehaviourValue(Type type, object source)
		{
			foreach (var childType in TypeUtils.GetBaseAndSelfTypes(type))
				SimpleCopyBehaviourValue(childType, source);
		}

		public static void SimpleCopyBehaviourValue(Type type, object source)
		{
			if (!TypeUtils.CheckIsBehaviourSubclass(type))
				return;

			if (TryPasteBehaviourValue(type, out var oldCopy) && oldCopy is Behaviour monoBehaviour)
				Object.DestroyImmediate(monoBehaviour.gameObject);

			BEHAVIOUR_VALUE_MAP_BY_TYPE.Remove(type);
			var copy = DuplicateBehaviour(source);
			if (copy == null)
				return;
			BEHAVIOUR_VALUE_MAP_BY_TYPE.Add(type, copy);
		}

		public static bool CheckCanPasteBehaviourValue(Type type) =>
			BEHAVIOUR_VALUE_MAP_BY_TYPE.ContainsKey(type);

		public static bool TryPasteBehaviourValue(Type type, out object copy)
		{
			if (!TypeUtils.CheckIsBehaviourSubclass(type) || !BEHAVIOUR_VALUE_MAP_BY_TYPE.TryGetValue(type, out var source))
			{
				copy = null;
				return false;
			}

			copy = DuplicateBehaviour(source);
			return copy != null;
		}

		public static object DuplicateBehaviour(object source)
		{
			if (source is not Behaviour sourceBehaviour)
				return null;
			var gameObject = sourceBehaviour.gameObject;
			var newGameObject = new GameObject(gameObject.name);
			newGameObject.SetActive(false);
			newGameObject.hideFlags = HideFlags.HideAndDontSave;
			var monoBehaviour = newGameObject.AddComponent(sourceBehaviour.GetType());
			OverrideBehaviour(sourceBehaviour, monoBehaviour);
			return monoBehaviour;
		}

		#endregion

		public static object Duplicate(Type sourceType, object source) =>
			Duplicate(sourceType, source, true);

		public static object Duplicate(Type sourceType, object source, bool isInitNullValues)
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
						element = isInitNullValues && TypeUtils.TryCreateInstance(elementType, out var localInstance) ? localInstance : null;
					}

					if (sourceType.IsArray)
						list[i] = element;
					else
						list.Add(element);
				}

				return list;
			}

			var newObj = isInitNullValues && TypeUtils.TryCreateInstance(sourceType, out var instance) ? instance : null;
			if (newObj == null)
				return null;
			OverrideObj(source, newObj);
			return newObj;
		}

		public static void OverrideBehaviour(object source, object newObj) =>
			EditorUtility.CopySerializedManagedFieldsOnly(source, newObj);

		public static void OverrideObj(object source, object newObj)
		{
			var json = EditorJsonUtility.ToJson(source);
			EditorJsonUtility.FromJsonOverwrite(json, newObj);
			RestoreUnityObjectReferences(source, newObj);
		}

		private static object RestoreUnityObjectReferences(object source, object destination)
		{
			if (source == null)
				return null;

			var sourceType = source.GetType();
			if (typeof(Object).IsAssignableFrom(sourceType))
				return source;

			if (destination == null)
				return null;

			if (source is IList sourceList && destination is IList destinationList)
			{
				var count = Math.Min(sourceList.Count, destinationList.Count);
				for (var i = 0; i < count; i++)
					destinationList[i] = RestoreUnityObjectReferences(sourceList[i], destinationList[i]);

				return destination;
			}

			if (sourceType.IsPrimitive || sourceType.IsEnum || sourceType == typeof(string))
				return destination;

			foreach (var type in TypeUtils.GetSelfAndBaseTypes(sourceType))
				foreach (var field in type.GetFields(SERIALIZED_FIELD_BINDINGS))
					if (CheckIsSerializedField(field))
					{
						var sourceValue = field.GetValue(source);
						var destinationValue = field.GetValue(destination);
						var restoredValue = RestoreUnityObjectReferences(sourceValue, destinationValue);
						field.SetValue(destination, restoredValue);
					}

			return destination;
		}

		private static bool CheckIsSerializedField(FieldInfo field) =>
			!field.IsStatic && !field.IsInitOnly && !field.IsNotSerialized && (field.IsPublic || field.IsDefined(typeof(SerializeField), true) || field.IsDefined(typeof(SerializeReference), true));

		public static void CopyToSystemClipboard(string source) =>
			GUIUtility.systemCopyBuffer = source;

		public static string PasteFromSystemClipboard() =>
			GUIUtility.systemCopyBuffer;
	}
}


