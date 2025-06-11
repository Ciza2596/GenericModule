using UnityEditor;

namespace CizaAudioModule.Editor.MapListVisual
{
	public abstract class BMapListDrawer : BCountBoxDrawer
	{
		protected override SerializedProperty ArrayProperty => Property.FindPropertyRelative("_maps");
		protected sealed override BBoxVE.IContent CreateBody() => CreateListVE();

		protected abstract BMapListVE CreateListVE();
	}
}