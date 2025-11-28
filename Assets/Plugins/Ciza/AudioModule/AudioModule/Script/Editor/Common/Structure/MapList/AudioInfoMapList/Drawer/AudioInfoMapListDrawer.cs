using CizaAudioModule.Editor.MapListVisual;
using UnityEditor;

namespace CizaAudioModule.Editor
{
	[CustomPropertyDrawer(typeof(AudioInfoMapList))]
	public class AudioInfoMapListDrawer : BMapListDrawer
	{
		protected override BMapListVE CreateMapListVE(SerializedProperty property, BoxVE root)
		{
			var listVE = new AudioInfoMapListVE(property, false);
			listVE.Initialize();
			return listVE;
		}
	}
}