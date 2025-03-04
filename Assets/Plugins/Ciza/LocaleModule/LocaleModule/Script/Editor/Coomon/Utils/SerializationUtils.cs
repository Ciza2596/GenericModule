﻿using System;
using System.Reflection;
using System.Linq;
using UnityEditor;
using System.Text.RegularExpressions;
using System.Collections;
using UnityEditor.SceneManagement;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEngine;

namespace CizaLocaleModule.Editor
{
	public static class SerializationUtils
	{
		// CONSTANTS: -----------------------------------------------------------------------------

		private const BindingFlags BINDINGS = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

		private static readonly Regex RX_ARRAY = new Regex(@"\[\d+\]");

		private const string SPACE = " ";


		// ENUMS: ---------------------------------------------------------------------------------

		public enum ChildrenMode
		{
			ShowLabelsInChildren,
			HideLabelsInChildren,
			FullWidthChildren
		}

		// UI TOOLKIT: ----------------------------------------------------------------------------

		public static bool CreateChildProperties(VisualElement root, SerializedProperty prop, ChildrenMode mode, float spaceHeight = 5, params string[] excludeFields)
		{
			var iteratorProperty = prop.Copy();
			var endProperty = iteratorProperty.GetEndProperty();

			var numProperties = 0;
			var next = iteratorProperty.NextVisible(true);
			if (!next)
				return false;

			if (spaceHeight > 0)
				root.Add(new VisualElement() { style = { height = spaceHeight } });

			root.Bind(prop.serializedObject);

			do
			{
				if (SerializedProperty.EqualContents(iteratorProperty, endProperty))
					break;

				if (excludeFields.Contains(iteratorProperty.name))
					continue;

				var field = mode switch
				{
					ChildrenMode.ShowLabelsInChildren => new PropertyField(iteratorProperty),
					ChildrenMode.HideLabelsInChildren => new PropertyField(iteratorProperty, SPACE),
					ChildrenMode.FullWidthChildren => new PropertyField(iteratorProperty, string.Empty),
					_ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
				};

				field.BindProperty(iteratorProperty);
				field.name = iteratorProperty.propertyPath;

				root.Add(field);
				numProperties += 1;
			} while (iteratorProperty.NextVisible(false));

			return numProperties != 0;
		}

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
			var field = obj?.GetType().GetField(fieldName, BINDINGS);
			if (field == null)
				return default;

			var value = field.GetValue(obj);
			if (value != null)
				return value;

			return TypeUtils.CreateInstance(field.FieldType);
		}

		private static object GetFieldValueWithIndex(string fieldName, object obj, int index)
		{
			var field = obj.GetType().GetField(fieldName, BINDINGS);
			if (field == null) return default;
			return field.GetValue(obj) is IList list ? list[index] : default;
		}
	}
}