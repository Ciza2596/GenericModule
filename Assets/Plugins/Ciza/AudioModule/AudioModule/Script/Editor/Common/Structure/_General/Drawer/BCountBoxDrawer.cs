using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace CizaAudioModule.Editor
{
	public abstract class BCountBoxDrawer : BBoxDrawer
	{
		protected virtual string CountText => "Count: ";

		protected sealed override VisualElement CreateHeadAdditional() => CreateListCountLabel();

		protected virtual Label CreateListCountLabel()
		{
			var arrayProperty = ArrayProperty;
			var label = new Label(CountText + arrayProperty.arraySize) { style = { paddingRight = 8 } };
			label.TrackPropertyValue(arrayProperty, property => label.text = CountText + property.arraySize);
			return label;
		}

		protected abstract SerializedProperty ArrayProperty { get; }
		protected abstract override BBoxVE.IContent CreateBody();
	}
}