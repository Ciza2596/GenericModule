using System;
using System.Reflection;
using System.Linq;
using UnityEditor;
using System.Text.RegularExpressions;
using System.Collections;
using UnityEditor.SceneManagement;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEngine;

namespace CizaAudioModule.Editor
{
	public static class SerializationUtils
	{
		// CONSTANTS: -----------------------------------------------------------------------------

		public const BindingFlags FIELD_BINDINGS = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

		public static readonly Regex RX_ARRAY = new Regex(@"\[\d+\]");

		public const string SPACE = " ";
		public const string SCRIPT_FIELD = "m_Script";

		public static readonly char[] ASSEMBLY_SEPARATOR = { ' ' };

		// ENUMS: ---------------------------------------------------------------------------------

		public enum ChildrenKinds
		{
			ShowLabelsInChildren,
			HideLabelsInChildren,
			FullWidthChildren
		}

		// UI TOOLKIT: ----------------------------------------------------------------------------

		#region GetType

		public static Type GetType(SerializedProperty property, bool isFullType)
		{
			if (property.propertyType != SerializedPropertyType.ManagedReference)
				return property.GetValue()?.GetType();

			if (isFullType)
			{
				var fullSplit = property.managedReferenceFullTypename.Split(ASSEMBLY_SEPARATOR);
				if (fullSplit.Length == 2)
					return Type.GetType(Assembly.CreateQualifiedName(fullSplit[0], fullSplit[1]));
			}

			var fieldSplit = property.managedReferenceFieldTypename.Split(ASSEMBLY_SEPARATOR);
			return fieldSplit.Length != 2 ? null : Type.GetType(Assembly.CreateQualifiedName(fieldSplit[0], fieldSplit[1]));
		}

		public static Type[] GetElementTypes(SerializedProperty property) =>
			TypeUtils.GetElementTypes(property.GetValue().GetType());

		#endregion

		public static void Duplicate(SerializedProperty property, object source)
		{
			if (source == null)
			{
				property.SetValue(null);
				return;
			}

			property.SetValue(CopyPasteUtils.Duplicate(source.GetType(), source));
		}

		#region CreateChildProperties

		public static bool CreateChildProperties(VisualElement root, SerializedProperty property, ChildrenKinds kind, float spaceHeight = 5, Action<SerializedPropertyChangeEvent> onChangeValue = null, params string[] excludeFields) =>
			CreateChildProperties(root, property, true, kind, spaceHeight, onChangeValue, excludeFields);

		private static bool CreateChildProperties(VisualElement root, SerializedProperty property, bool isUsedEnd, ChildrenKinds kind, float spaceHeight = 5, Action<SerializedPropertyChangeEvent> onChangeValue = null, params string[] excludeFields)
		{
			var iteratorProperty = property.Copy();
			var endProperty = isUsedEnd ? iteratorProperty.GetEndProperty() : null;
			var isNext = iteratorProperty.NextVisible(true);
			if (!isNext)
				return false;

			if (spaceHeight > 0)
				root.Add(new VisualElement() { style = { height = spaceHeight } });

			root.Bind(property.serializedObject);
			var propertyNumber = 0;

			do
			{
				if (isUsedEnd && SerializedProperty.EqualContents(iteratorProperty, endProperty))
					break;

				if (iteratorProperty.name == SCRIPT_FIELD || excludeFields.Contains(iteratorProperty.name))
					continue;

				var fieldVE = kind switch
				{
					ChildrenKinds.ShowLabelsInChildren => new PropertyField(iteratorProperty),
					ChildrenKinds.HideLabelsInChildren => new PropertyField(iteratorProperty, SPACE),
					ChildrenKinds.FullWidthChildren => new PropertyField(iteratorProperty, string.Empty),
					_ => throw new ArgumentOutOfRangeException(nameof(kind), kind, null)
				};

				fieldVE.BindProperty(iteratorProperty);
				if (onChangeValue != null)
					fieldVE.RegisterValueChangeCallback(onChangeValue.Invoke);
				fieldVE.name = iteratorProperty.propertyPath;

				root.Add(fieldVE);
				propertyNumber += 1;
			} while (iteratorProperty.NextVisible(false));

			return propertyNumber != 0;
		}

		#endregion

		// UPDATE SERIALIZATION: ------------------------------------------------------------------

		public static void ApplyUnregisteredSerialization(SerializedObject serializedObject)
		{
			serializedObject.ApplyModifiedProperties();
			serializedObject.Update();
			serializedObject.SetIsDifferentCacheDirty();

			var component = serializedObject.targetObject as Component;
			if (component == null || !component.gameObject.scene.isLoaded)
				return;

			if (Application.isPlaying)
				return;
			EditorSceneManager.MarkSceneDirty(component.gameObject.scene);
		}

		// GET MANAGED REFERENCES: ----------------------------------------------------------------

		public static object GetValue(this SerializedProperty property) => GetValue<object>(property);

		public static T GetValue<T>(this SerializedProperty property)
		{
			// Now that Unity supports managedReferenceValue 'getters' use it by default.
			// However there is no way at the moment to get the value of a generic object
			// so instead, use the object-path traverse method.

			// Update 5/2/2022: There is a new boxed object value property available inside the
			// SerializedProperty class. Might be what we are looking for.
			// Resolution: Negative. It would work, but if the boxed value contains any
			// UnityEngine.Object reference the deserialization fails and throws an exception.

			// Update 18/9/2023: Since Unity has provided much more support for serialized
			// references it is clear that generic data should never be accessed and modified
			// as-is. Therefore all generic data should be converted to managed reference values.

			if (property == null) return default;
			ApplyUnregisteredSerialization(property.serializedObject);

			if (property.propertyType == SerializedPropertyType.ManagedReference)
				return property.managedReferenceValue is T managedReference ? managedReference : default;

			object obj = property.serializedObject.targetObject;
			var path = property.propertyPath.Replace(".Array.data[", "[");

			var fieldStructure = path.Split('.');
			foreach (var field in fieldStructure)
			{
				if (field.Contains("["))
				{
					var groups = RX_ARRAY.Match(field).Groups;
					var index = int.Parse(groups[0].Value.Replace("[", "").Replace("]", ""));
					obj = GetFieldValueWithIndex(RX_ARRAY.Replace(field, string.Empty), obj, index);
				}
				else
					obj = GetFieldValue(field, obj);
			}

			return (T)obj;
		}

		public static void SetValue(this SerializedProperty property, object value)
		{
			if (property.propertyType == SerializedPropertyType.ManagedReference)
				property.managedReferenceValue = value;
			else if (value is IList list)
			{
				property.arraySize = 0;
				for (var i = 0; i < list.Count; i++)
				{
					property.InsertArrayElementAtIndex(i);
					property.GetArrayElementAtIndex(i).SetValue(list[i]);
				}
			}
			else
				property.boxedValue = value;

			ApplyUnregisteredSerialization(property.serializedObject);
		}

		private static object GetFieldValue(string fieldName, object obj)
		{
			var fieldInfo = obj?.GetType().GetField(fieldName, FIELD_BINDINGS);
			if (fieldInfo == null)
				return null;

			var value = fieldInfo.GetValue(obj);
			if (value != null)
				return value;

			TypeUtils.TryCreateInstance(fieldInfo.FieldType, out var instance);
			return instance;
		}

		private static object GetFieldValueWithIndex(string fieldName, object obj, int index)
		{
			var fieldInfo = obj.GetType().GetField(fieldName, FIELD_BINDINGS);
			if (fieldInfo == null) return null;
			return fieldInfo.GetValue(obj) is IList list ? list[index] : null;
		}
	}
}