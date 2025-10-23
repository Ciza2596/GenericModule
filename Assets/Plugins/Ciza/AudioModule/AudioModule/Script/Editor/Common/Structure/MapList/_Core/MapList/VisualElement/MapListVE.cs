using UnityEditor;
using UnityEngine.Scripting;

namespace CizaAudioModule.Editor.MapListVisual
{
	public class MapListVE: BMapListVE
	{
		// VARIABLE: -----------------------------------------------------------------------------

		protected readonly string _keyLabel;
		protected readonly string _valueLabel;

		// CONSTRUCTOR: --------------------------------------------------------------------- 

		[Preserve]
		public MapListVE(string keyLabel, string valueLabel, SerializedProperty listProperty) : base(listProperty)
		{
			_keyLabel = keyLabel;
			_valueLabel = valueLabel;
		}

		// PROTECT METHOD: --------------------------------------------------------------------

		protected override void DerivedInitialize()
		{
			base.DerivedInitialize();
			Refresh();
		}

		protected override ItemVE CreateItemVE(SerializedProperty itemProperty)
		{
			var itemVE = new MapItemVE(_keyLabel, _valueLabel, this, itemProperty);
			itemVE.Initialize();
			return itemVE;
		}
	}
}