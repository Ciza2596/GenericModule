using System;
using UnityEngine.UIElements;

namespace CizaAudioModule.Editor
{
	public class AlignLabel
	{
		// PUBLIC CONSTANTS: ----------------------------------------------------------------------

		public const string UNITY_INSPECTOR_CLASS = "unity-inspector-main-container";
		public const string UNITY_INSPECTOR_ELEMENT_CLASS = "unity-inspector-element";
		
		public const string UNITY_ALIGN_FIELD_CLASS = "unity-base-field__aligned";

		// PRIVATE CONSTANTS: ---------------------------------------------------------------------

		private const float EPSILON = 0.001f;

		private static readonly CustomStyleProperty<float> LABEL_WIDTH_RATIO_PROPERTY = new CustomStyleProperty<float>("--unity-property-field-label-width-ratio");
		private static readonly CustomStyleProperty<float> LABEL_EXTRA_PADDING_PROPERTY = new CustomStyleProperty<float>("--unity-property-field-label-extra-padding");
		private static readonly CustomStyleProperty<float> LABEL_BASE_MIN_WIDTH_PROPERTY = new CustomStyleProperty<float>("--unity-property-field-label-base-min-width");
		private static readonly CustomStyleProperty<float> LABEL_EXTRA_CONTEXT_WIDTH_PROPERTY = new CustomStyleProperty<float>("--unity-base-field-extra-context-width");

		private const float LABEL_WIDTH_RATION = 0.45f;
		private const float LABEL_EXTRA_PADDING = 37f;
		private const float LABEL_BASE_MIN_WIDTH = 123f;
		private const float LABEL_EXTRA_CONTEXT_WIDTH = 1f;

		// MEMBERS: -------------------------------------------------------------------------------

		[NonSerialized]
		private readonly VisualElement _field;

		[NonSerialized]
		private readonly Label _label;

		[NonSerialized]
		private float _labelWidthRatio;

		[NonSerialized]
		private float _labelExtraPadding;

		[NonSerialized]
		private float _labelBaseMinWidth;

		[NonSerialized]
		private float _labelExtraContextWidth;

		[NonSerialized]
		private VisualElement _cachedContextWidthElement;

		[NonSerialized]
		private VisualElement _cachedInspectorElement;

		// CONSTRUCTOR: ---------------------------------------------------------------------------

		private AlignLabel(VisualElement field)
		{
			_field = field;
			_label = field.Q<Label>();

			_field.RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);
			_field.RegisterCallback<DetachFromPanelEvent>(OnDetachFromPanel);
		}

		// PUBLIC METHODS: ------------------------------------------------------------------------

		public static void On(VisualElement field)
		{
			_ = new AlignLabel(field);
		}

		// CALLBACKS: -----------------------------------------------------------------------------

		private void OnAttachToPanel(AttachToPanelEvent eventAttach)
		{
			if (eventAttach.destinationPanel == null) return;
			if (eventAttach.destinationPanel.contextType == ContextType.Player) return;

			var currentElement = _field.parent;
			while (currentElement != null)
			{
				if (currentElement.ClassListContains(UNITY_INSPECTOR_ELEMENT_CLASS))
					_cachedInspectorElement = currentElement;

				if (currentElement.ClassListContains(UNITY_INSPECTOR_CLASS))
				{
					_cachedContextWidthElement = currentElement;
					break;
				}

				currentElement = currentElement.parent;
			}

			if (_cachedInspectorElement == null)
				return;

			_labelWidthRatio = LABEL_WIDTH_RATION;
			_labelExtraPadding = LABEL_EXTRA_PADDING;
			_labelBaseMinWidth = LABEL_BASE_MIN_WIDTH;
			_labelExtraContextWidth = LABEL_EXTRA_CONTEXT_WIDTH;

			_field.RegisterCallback<CustomStyleResolvedEvent>(OnCustomStyleResolved);
			_field.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
		}

		private void OnDetachFromPanel(DetachFromPanelEvent eventDetach)
		{
			_field.UnregisterCallback<CustomStyleResolvedEvent>(OnCustomStyleResolved);
			_field.UnregisterCallback<GeometryChangedEvent>(OnGeometryChanged);
		}

		private void OnCustomStyleResolved(CustomStyleResolvedEvent eventResolution)
		{
			if (eventResolution.customStyle.TryGetValue(LABEL_WIDTH_RATIO_PROPERTY, out var labelWidthRatio))
				_labelWidthRatio = labelWidthRatio;

			if (eventResolution.customStyle.TryGetValue(LABEL_EXTRA_PADDING_PROPERTY, out var labelExtraPadding))
				_labelExtraPadding = labelExtraPadding;

			if (eventResolution.customStyle.TryGetValue(LABEL_BASE_MIN_WIDTH_PROPERTY, out var labelBaseMinWidth))
				_labelBaseMinWidth = labelBaseMinWidth;

			if (eventResolution.customStyle.TryGetValue(LABEL_EXTRA_CONTEXT_WIDTH_PROPERTY, out var labelExtraContextWidth))
				_labelExtraContextWidth = labelExtraContextWidth;

			ChangeWidth();
		}

		private void OnGeometryChanged(GeometryChangedEvent eventGeometry)
		{
			ChangeWidth();
		}

		// PRIVATE METHODS: -----------------------------------------------------------------------

		private void ChangeWidth()
		{
			var totalPadding = _labelExtraPadding;
			var spacing = _field.worldBound.x - _cachedInspectorElement.worldBound.x - _cachedInspectorElement.resolvedStyle.paddingLeft;

			totalPadding += spacing;
			totalPadding += _field.resolvedStyle.paddingLeft;

			var minWidth = _labelBaseMinWidth - spacing - _field.resolvedStyle.paddingLeft;
			var contextWidthElement = _cachedContextWidthElement ?? _cachedInspectorElement;

			_label.style.minWidth = Math.Max(minWidth, 0);

			var newWidth = (contextWidthElement.resolvedStyle.width + _labelExtraContextWidth) * _labelWidthRatio - totalPadding;
			if (Math.Abs(_label.resolvedStyle.width - newWidth) > EPSILON)
				_label.style.width = Math.Max(0f, newWidth);
		}
	}
}