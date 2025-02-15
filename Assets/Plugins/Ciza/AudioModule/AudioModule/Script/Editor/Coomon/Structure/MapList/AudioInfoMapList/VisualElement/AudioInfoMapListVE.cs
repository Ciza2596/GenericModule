using CizaAudioModule.Editor.MapListVisual;
using UnityEditor;
using UnityEngine.Scripting;

namespace CizaAudioModule.Editor
{
	public class AudioInfoMapListVE : BMapListVE
	{
		[Preserve]
		public AudioInfoMapListVE(SerializedProperty mapListProperty) : base(mapListProperty) { }

		protected override void DerivedInitialize()
		{
			base.DerivedInitialize();
			Refresh();
		}

		protected override BMapItemVE CreateMapItem(SerializedProperty mapListProperty)
		{
			var addressMapItemVE = new AudioInfoMapItemVE(this, mapListProperty);
			addressMapItemVE.Initialize();
			return addressMapItemVE;
		}
	}
}