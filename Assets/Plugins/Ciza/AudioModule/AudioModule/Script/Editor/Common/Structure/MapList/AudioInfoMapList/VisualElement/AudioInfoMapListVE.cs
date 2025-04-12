using CizaAudioModule.Editor.MapListVisual;
using UnityEditor;
using UnityEngine.Scripting;

namespace CizaAudioModule.Editor
{
	public class AudioInfoMapListVE : BMapListVE
	{
		// CONSTRUCTOR: --------------------------------------------------------------------- 

		[Preserve]
		public AudioInfoMapListVE(SerializedProperty listProperty) : base(listProperty) { }

		// PROTECT METHOD: --------------------------------------------------------------------

		protected override void DerivedInitialize()
		{
			base.DerivedInitialize();
			Refresh();
		}

		protected override ItemVE CreateItem(SerializedProperty itemProperty)
		{
			var itemVE = new AudioInfoMapItemVE(this, itemProperty);
			itemVE.Initialize();
			return itemVE;
		}
	}
}