using CizaAudioModule.Editor.MapListVisual;
using UnityEditor;

namespace CizaAudioModule.Editor
{
	[CustomPropertyDrawer(typeof(AudioInfoMapList))]
	public class AudioInfoMapListDrawer : BMapListDrawer
	{
		protected override BMapListVE CreateListVE()
		{
			var listVE = new AudioInfoMapListVE(Property);
			listVE.Initialize();
			return listVE;
		}
	}
}