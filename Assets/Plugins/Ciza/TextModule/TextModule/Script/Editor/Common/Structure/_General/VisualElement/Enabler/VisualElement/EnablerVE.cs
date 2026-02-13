using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.Scripting;
using UnityEngine.UIElements;

namespace CizaTextModule.Editor
{
	public class EnablerVE : VisualElement
	{
		// VARIABLE: -----------------------------------------------------------------------------

		[field: NonSerialized]
		protected readonly VisualElement _valueContainer = new VisualElement();

		[field: NonSerialized]
		protected Toggle _isEnableToggle;

		protected virtual string[] USSPaths => new[] { "Enabler" };

		protected virtual string[] EnablerClasses => new[] { "enabler" };
		protected virtual string[] ToggleClasses => new[] { AlignLabel.UNITY_ALIGN_FIELD_CLASS };

		protected virtual string[] ValueContainerClasses => new[] { "value-container" };

		protected virtual string IsEnablePath => "_isEnable";
		protected virtual string ValuePath => "_value";

		[field: NonSerialized]
		protected virtual SerializedProperty EnablerProperty { get; }

		[field: NonSerialized]
		protected virtual Type ValueType { get; }

		protected virtual SerializedProperty IsEnableProperty => EnablerProperty.FindPropertyRelative(IsEnablePath);
		protected virtual SerializedProperty ValueProperty => EnablerProperty.FindPropertyRelative(ValuePath);

		// PUBLIC VARIABLE: ---------------------------------------------------------------------

		[field: NonSerialized]
		public bool IsInitialized { get; protected set; }


		// CONSTRUCTOR: ------------------------------------------------------------------------

		[Preserve]
		public EnablerVE(SerializedProperty property)
		{
			EnablerProperty = property;
			ValueType = SerializationUtils.GetElementTypes(EnablerProperty)[0];
		}


		// PUBLIC METHOD: ----------------------------------------------------------------------

		public void Initialize()
		{
			if (IsInitialized)
				return;
			IsInitialized = true;

			foreach (var sheet in StyleSheetUtils.GetStyleSheets(USSPaths))
				styleSheets.Add(sheet);

			foreach (var c in EnablerClasses)
				AddToClassList(c);

			RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);

			DerivedInitialize();
		}


		// PROTECT METHOD: --------------------------------------------------------------------

		protected virtual void DerivedInitialize()
		{
			_isEnableToggle = new Toggle(EnablerProperty.displayName);
			foreach (var c in ToggleClasses)
				_isEnableToggle.AddToClassList(c);
			_isEnableToggle.BindProperty(IsEnableProperty);
			Add(_isEnableToggle);

			foreach (var c in ValueContainerClasses)
				_valueContainer.AddToClassList(c);

			_isEnableToggle.RegisterValueChangedCallback(changeEvent => { _valueContainer.style.display = changeEvent.newValue ? DisplayStyle.Flex : DisplayStyle.None; });
			Add(_valueContainer);

			if (TypeUtils.CheckIsClassWithoutStringOrUnityObjSubclass(ValueType))
			{
				style.flexDirection = FlexDirection.Column;
				SerializationUtils.CreateChildProperties(_valueContainer, ValueProperty, SerializationUtils.ChildrenKinds.ShowLabelsInChildren, 0);
			}
			else
			{
				style.flexDirection = FlexDirection.Row;
				SetupField(ValueProperty, false);
			}
		}

		protected virtual void OnGeometryChanged(GeometryChangedEvent evt)
		{
			foreach (var c in EnablerClasses)
				_isEnableToggle.EnableInClassList(c, false);
			foreach (var c in EnablerClasses)
				_isEnableToggle.EnableInClassList(c, true);
		}

		protected virtual void SetupField(SerializedProperty property, bool hasLabel = true)
		{
			var field = new PropertyField(property);
			field.BindProperty(property);
			if (!hasLabel) field.label = string.Empty;
			_valueContainer.Add(field);
		}
	}
}