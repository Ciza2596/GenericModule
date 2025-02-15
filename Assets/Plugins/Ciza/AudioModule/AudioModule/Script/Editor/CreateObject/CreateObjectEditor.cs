using UnityEditor;
using UnityEngine;

namespace CizaAudioModule.Editor
{
    public static class CreateObjectEditor
    {
        public const string AUDIO_MODULE_PATH = "AudioModule/Prefab/Obj/";

        public const string AUDIO = "Audio";

        [MenuItem("GameObject/Ciza/AudioModule/Audio", false, -602)]
        public static void CreateLoadUi() =>
            CreateObject(AUDIO);

        private static void CreateObject(string dataId)
        {
            var prefab = Resources.Load<GameObject>(AUDIO_MODULE_PATH + dataId);
            var uiObject = Object.Instantiate(prefab, Selection.activeTransform);
            uiObject.name = dataId;
        }
    }
}