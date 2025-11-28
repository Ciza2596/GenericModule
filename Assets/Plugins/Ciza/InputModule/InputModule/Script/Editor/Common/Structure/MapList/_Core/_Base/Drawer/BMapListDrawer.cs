using UnityEditor;

namespace CizaInputModule.Editor.MapListVisual
{
	public abstract class BMapListDrawer : BListBoxDrawer
	{
		protected override SerializedProperty GetItemsProperty(SerializedProperty property, BoxVE root) => property.FindPropertyRelative("_maps");


		protected sealed override ListVE CreateListVE(SerializedProperty property, BoxVE root) =>
			CreateMapListVE(property, root);

		protected abstract BMapListVE CreateMapListVE(SerializedProperty property, BoxVE root);
	}
}