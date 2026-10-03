using System;
using System.Reflection;
using System.Linq;
using UnityEditor;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CizaAddressablesModule.Editor
{
	public static class SerializationUtils
	{
		// CONSTANTS: -----------------------------------------------------------------------------

		public const BindingFlags FIELD_BINDINGS = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
		public const BindingFlags DECLARED_ONLY_FIELD_BINDINGS = FIELD_BINDINGS | BindingFlags.DeclaredOnly;

		public static readonly Regex RX_ARRAY = new Regex(@"\[\d+\]");
		public static readonly Regex RX_ARRAY_PATH = new Regex(@"Array\.data\[\d+\]$");

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

		#region Path

		public static bool TryGetParentPath(SerializedProperty property, out string parentPath) =>
			TryGetParentPath(property.propertyPath, out parentPath);

		public static bool TryGetParentPath(string path, out string parentPath)
		{
			var index = path.LastIndexOf('.');

			if (index < 0)
			{
				parentPath = string.Empty;
				return false;
			}

			if (path[(index + 1)..].Contains("["))
				index = path[..index].LastIndexOf('.');

			parentPath = path[..index];
			return true;
		}


		public static string GetSimplePath(SerializedProperty property) =>
			GetSimplePath(property.propertyPath);

		public static string GetSimplePath(string path)
		{
			if (string.IsNullOrEmpty(path))
				return path;

			if (RX_ARRAY_PATH.IsMatch(path))
			{
				int last = path.LastIndexOf('.');
				if (last < 0) return path;

				int secondLast = path.LastIndexOf('.', last - 1);
				if (secondLast < 0) return path;

				int thirdLast = path.LastIndexOf('.', secondLast - 1);
				if (thirdLast < 0) return path.Substring(0, secondLast);

				return path.Substring(0, thirdLast);
			}

			int dot = path.LastIndexOf('.');
			if (dot >= 0)
				return path.Substring(dot + 1);

			return path;
		}

		#endregion

		#region Property

		public static bool TryGetParentProperty(SerializedProperty property, out SerializedProperty parentProperty)
		{
			if (!TryGetParentPath(property, out var parentPath))
			{
				parentProperty = null;
				return false;
			}

			parentProperty = property.serializedObject.FindProperty(parentPath);
			return true;
		}

		#endregion

		public static string GetTitle(SerializedProperty property, bool isFullType, string[] forbiddenNames = null) =>
			TypeUtils.GetTitle(GetType(property, isFullType), forbiddenNames);

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

		public static Type GetParentInstanceType(SerializedProperty property) =>
			GetParentInstance(property).GetType();

		public static Type[] GetElementTypes(SerializedProperty property) =>
			TypeUtils.GetElementTypes(property.GetValue().GetType());

		#endregion

		#region CheckHasAttribute

		public static bool CheckHasAttribute<TAttribute>(this SerializedProperty property, out int count, bool inherit = true) where TAttribute : Attribute =>
			CheckHasAttributeOnField<TAttribute>(property, out count, inherit) || CheckHasAttributeOnType<TAttribute>(property, out count, inherit);

		public static bool CheckHasAttributeOnField<TAttribute>(this SerializedProperty property, out int count, bool inherit = true) where TAttribute : Attribute
		{
			if (property == null)
			{
				count = 0;
				return false;
			}

			var parentInstanceType = GetParentInstanceType(property);
			var fieldInfo = parentInstanceType.GetField(GetSimplePath(property), FIELD_BINDINGS);
			count = fieldInfo?.GetCustomAttributes(typeof(TAttribute), inherit).Length ?? 0;
			return count > 0;
		}

		public static bool CheckHasAttributeOnType<TAttribute>(this SerializedProperty property, out int count, bool inherit = true) where TAttribute : Attribute
		{
			var hasAttribute = GetType(property, false) is { } type && type.IsDefined(typeof(TAttribute), inherit);
			count = hasAttribute ? 1 : 0;
			return hasAttribute;
		}

		#endregion

		public static object GetParentInstance(this SerializedProperty property)
		{
			if (TryGetParentPath(property, out var parentPath))
				return property.serializedObject.FindProperty(parentPath).GetValue();
			return property.serializedObject.targetObject;
		}

		public static void Duplicate(SerializedProperty property, object source)
		{
			if (source == null)
			{
				property.SetValue(null);
				return;
			}

			property.SetValue(CopyPasteUtils.Duplicate(source.GetType(), source));
		}

		#region Create Field

		public static PopupField<T> CreatePopupField<T>(List<T> options, SerializedProperty property, bool isFull, Action<T> onChangeValue = null) =>
			CreatePopupField(options, property, !property.CheckHasAttribute<HideLabelAttribute>(out _), isFull, onChangeValue);


		public static PopupField<T> CreatePopupField<T>(List<T> options, SerializedProperty property, bool isShowLabel, bool isFull, Action<T> onChangeValue = null)
		{
			T defaultValue = property.GetValue<string>().CheckHasValue() && options.Contains(property.GetValue<T>()) ? property.GetValue<T>() : options[0];
			var popupField = new PopupField<T>(options, defaultValue) { label = isShowLabel ? property.displayName : (isFull ? string.Empty : SPACE) };
			property.SetValue(defaultValue);
			popupField.AddToClassList(AlignLabel.UNITY_ALIGN_FIELD_CLASS);

			popupField.RegisterValueChangedCallback(evt =>
			{
				property.SetValue(evt.newValue);
				onChangeValue?.Invoke(evt.newValue);
			});
			popupField.BindProperty(property);
			return popupField;
		}

		public static PropertyField CreatePropertyField(SerializedProperty property, bool isFull) =>
			CreatePropertyField(property, !property.CheckHasAttribute<HideLabelAttribute>(out _), isFull);

		public static PropertyField CreatePropertyField(SerializedProperty property, bool isShowLabel, bool isFull)
		{
			var propertyField = isShowLabel ? new PropertyField(property) : new PropertyField(property, isFull ? string.Empty : SPACE);
			propertyField.BindProperty(property);
			return propertyField;
		}

		public static ToggleButtonGroup CreateToggleButtonGroup(SerializedProperty property, bool isWrapped, bool isStretched)
		{
			var type = GetType(property, true);
			var isFlags = type is { IsEnum: true } && type.IsDefined(typeof(FlagsAttribute), false);
			var toggleButtonGroup = new ToggleButtonGroup(property.displayName) { allowEmptySelection = false, isMultipleSelection = isFlags };
			toggleButtonGroup.AddToClassList(AlignLabel.UNITY_ALIGN_FIELD_CLASS);
			toggleButtonGroup.contentContainer.style.flexWrap = isWrapped ? Wrap.Wrap : Wrap.NoWrap;

			var isStretchedIndex = isStretched ? 1 : 0;
			foreach (var enumName in property.enumDisplayNames)
				toggleButtonGroup.Add(new Button { text = enumName, style = { flexShrink = 1, flexGrow = isStretchedIndex } });

			RefreshField();
			toggleButtonGroup.RegisterValueChangedCallback(OnToggleButtonGroupChangeValue);
			toggleButtonGroup.TrackPropertyValue(property, _ => RefreshField());
			return toggleButtonGroup;

			void OnToggleButtonGroupChangeValue(ChangeEvent<ToggleButtonGroupState> @event)
			{
				var value = @event.newValue;
				var indices = value.GetActiveOptions(stackalloc int[value.length]).ToArray();

				if (isFlags)
				{
					var intValue = 0;
					foreach (var index in indices)
						intValue |= 1 << index;

					property.enumValueFlag = intValue;
				}
				else
				{
					var index = indices.FirstOrDefault();
					property.enumValueIndex = index;
				}

				ApplyUnregisteredSerialization(property.serializedObject);

				RefreshField();
			}

			void RefreshField()
			{
				var state = toggleButtonGroup.value;

				if (isFlags)
				{
					var length = property.enumNames.Length;
					for (int i = 0; i < length; i++)
						state[i] = (property.enumValueFlag & (1 << i)) != 0;
				}
				else
				{
					state.ResetAllOptions();
					var enumValueIndex = property.enumValueIndex > 0 ? property.enumValueIndex : 0;
					state[enumValueIndex] = true;
				}

				toggleButtonGroup.SetValueWithoutNotify(state);
			}
		}

		#endregion


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


		public static List<SwitchGroup> GetChildrenGroupList(this SerializedProperty property, bool inherit = false, int startIndex = -1)
		{
			var result = new List<SwitchGroup>();
			var type = GetType(property, true);
			if (type == null)
				return result;

			var fieldInfos = new List<FieldInfo>();

			if (inherit)
				foreach (var baseType in TypeUtils.GetBaseTypes(type))
					fieldInfos.AddRange(baseType.GetFields(DECLARED_ONLY_FIELD_BINDINGS));

			fieldInfos.AddRange(type.GetFields(DECLARED_ONLY_FIELD_BINDINGS));

			var index = startIndex;
			foreach (var fieldInfo in fieldInfos)
			{
				if (fieldInfo.IsDefined(typeof(SerializeField), inherit) && !fieldInfo.IsDefined(typeof(HideInInspector)))
					index++;
				else
					continue;

				if (fieldInfo.GetCustomAttribute<SwitchGroupAttribute>(inherit) is not { } attribute)
					continue;

				var childProperty = property.FindPropertyRelative(fieldInfo.Name);
				var associatedValue = attribute.GetAssociatedValue();

				if (result.FirstOrDefault(switchGroup => switchGroup.GroupName == attribute.GetGroupName()) is { } group)
				{
					if (attribute.IsLeader)
					{
						group.SetLeaderProperty(childProperty);
						group.SetSiblingIndex(index);
					}
					else
					{
						index--;
						group.AddMember(childProperty, associatedValue, attribute.IsInverted);
					}
				}
				else
				{
					var newGroup = new SwitchGroup(attribute.GetGroupName());
					if (attribute.IsLeader)
					{
						newGroup.SetLeaderProperty(childProperty);
						newGroup.SetSiblingIndex(index);
					}
					else
					{
						index--;
						newGroup.AddMember(childProperty, associatedValue, attribute.IsInverted);
					}

					result.Add(newGroup);
				}
			}

			return result;
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


