using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace CizaAudioModule.Editor.MapListVisual
{
	public abstract class BMapListDrawer : BBoxDrawer
	{
		protected virtual string CountText => "Count: ";

		protected sealed override VisualElement CreateHeadAdditional() => CreateListCountLabel();

		protected sealed override BBoxVE.IContent CreateBody() => CreateMapListVE();

		protected virtual Label CreateListCountLabel()
		{
			var mapsProperty = Property.FindPropertyRelative("_maps");
			var label = new Label(CountText + mapsProperty.arraySize) { style = { paddingRight = 8 } };
			label.TrackPropertyValue(mapsProperty, property => label.text = CountText + property.arraySize);
			return label;
		}

		protected abstract BMapListVE CreateMapListVE();
	}
}