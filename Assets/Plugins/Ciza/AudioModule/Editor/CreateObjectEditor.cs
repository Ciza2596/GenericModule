using UnityEditor;
using UnityEngine;

namespace CizaAudioModule.Editor
{
    public class CreateObjectEditor
    {
        public const string AudioModulePath = "AudioModule/";

        public const string Audio = "Audio";

        [MenuItem("GameObject/Ciza/AudioModule/Audio", false, -602)]
        public static void CreateLoadUi() =>
            CreateObject(Audio);

        private static void CreateObject(string dataId)
        {
            var prefab = Resources.Load<GameObject>(AudioModulePath + dataId);
            var uiObject = Object.Instantiate(prefab, Selection.activeTransform);
            uiObject.name = dataId;
        }
    }
}