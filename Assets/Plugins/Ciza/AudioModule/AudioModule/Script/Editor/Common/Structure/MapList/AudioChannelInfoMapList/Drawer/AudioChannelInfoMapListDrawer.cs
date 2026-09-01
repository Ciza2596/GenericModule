using CizaAudioModule.Editor.MapListVisual;
using UnityEditor;

namespace CizaAudioModule.Editor
{
	[CustomPropertyDrawer(typeof(AudioChannelInfoMapList))]
	public class AudioChannelInfoMapListDrawer : MapListDrawer
	{
		protected override string KeyLabel => "DataId";
	}
}