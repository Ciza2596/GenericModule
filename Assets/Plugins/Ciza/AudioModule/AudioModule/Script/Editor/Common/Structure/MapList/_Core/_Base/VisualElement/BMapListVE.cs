using UnityEditor;
using UnityEngine.Scripting;

namespace CizaAudioModule.Editor.MapListVisual
{
	public abstract class BMapListVE : ListVE
	{
		// PUBLIC VARIABLE: ---------------------------------------------------------------------

		public override bool IsAllowDisable => true;

		// CONSTRUCTOR: ---------------------------------------------------------------------------

		[Preserve]
		protected BMapListVE(SerializedProperty listProperty, bool isAutoRefresh) : base(listProperty, isAutoRefresh) { }

		// PROTECT METHOD: --------------------------------------------------------------------

		protected override SerializedProperty CreateItemsProperty() =>
			ListProperty.FindPropertyRelative("_maps");

		protected abstract override ItemVE CreateItemVE(SerializedProperty itemProperty);
	}
}