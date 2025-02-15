using CizaAudioModule.Editor.MapListVisual;
using UnityEditor;

namespace CizaAudioModule.Editor
{
	[CustomPropertyDrawer(typeof(AudioInfoMapList))]
	public class AudioInfoMapListDrawer : BMapListDrawer
	{
		protected override BMapListVE CreateMapListVE()
		{
			var addressMapListVE = new AudioInfoMapListVE(Property);
			addressMapListVE.Initialize();
			return addressMapListVE;
		}
	}
}