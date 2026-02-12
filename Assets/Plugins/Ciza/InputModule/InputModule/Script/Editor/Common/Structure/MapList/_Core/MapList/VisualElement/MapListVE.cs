using UnityEditor;
using UnityEngine.Scripting;

namespace CizaInputModule.Editor.MapListVisual
{
	public class MapListVE : BMapListVE
	{
		// VARIABLE: -----------------------------------------------------------------------------

		protected readonly string _keyLabel;
		protected readonly string _valueLabel;

		// CONSTRUCTOR: --------------------------------------------------------------------- 

		[Preserve]
		public MapListVE(SerializedProperty listProperty, bool isAutoRefresh, string keyLabel, string valueLabel) : base(listProperty, isAutoRefresh)
		{
			_keyLabel = keyLabel;
			_valueLabel = valueLabel;
		}

		// PROTECT METHOD: --------------------------------------------------------------------

		protected override ItemVE CreateItemVE(SerializedProperty itemProperty)
		{
			var itemVE = new MapItemVE(this, itemProperty, _keyLabel, _valueLabel);
			itemVE.Initialize();
			return itemVE;
		}
	}
}