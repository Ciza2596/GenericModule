using CizaAudioModule.Editor.MapListVisual;
using UnityEditor;

namespace CizaAudioModule.Editor
{
    [CustomPropertyDrawer(typeof(AudioInfoMapList.Map))]
    public class AudioInfoMapDrawer : BMapDrawer { }
}