using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace CizaAudioModule.Editor
{
	public abstract class BListBoxDrawer : BBoxDrawer
	{
		// VARIABLE: -----------------------------------------------------------------------------

		protected virtual string CountText => "Count: ";

		protected abstract SerializedProperty GetItemsProperty(SerializedProperty property, BoxVE root);

		// PROTECT METHOD: --------------------------------------------------------------------
		protected sealed override VisualElement CreateHeadAdditional(SerializedProperty property, BoxVE root) => CreateListCountLabel(property, root);

		protected virtual Label CreateListCountLabel(SerializedProperty property, BoxVE root)
		{
			var itemsProperty = GetItemsProperty(property, root);
			var label = new Label(CountText + itemsProperty.arraySize) { style = { paddingRight = 8 } };
			label.TrackPropertyValue(itemsProperty, itemsProperty_ =>
			{
				label.text = CountText + itemsProperty_.arraySize;
				root.Refresh();
			});
			return label;
		}

		protected override BBoxVE.IContent CreateBody(SerializedProperty property, BoxVE root) =>
			CreateListVE(property, root);

		protected abstract ListVE CreateListVE(SerializedProperty property, BoxVE root);
	}
}