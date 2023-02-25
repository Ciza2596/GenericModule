using UnityEditor;
using UnityEngine;

namespace SaveLoadModule.Editor
{
    public class OpenPersistentDataPathEditor : EditorWindow
    {
        [MenuItem("Tools/CizaModule/OpenPersistentDataPath", false, 10000)]
        private static void OpenPersistentDataPath() =>
            EditorUtility.RevealInFinder(Application.persistentDataPath);
    }
}