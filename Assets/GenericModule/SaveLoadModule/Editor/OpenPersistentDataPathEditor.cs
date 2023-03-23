using UnityEditor;
using UnityEngine;

namespace CizaSaveLoadModule.Editor
{
    public class OpenPersistentDataPathEditor : EditorWindow
    {
        [MenuItem("Tools/Ciza/OpenPersistentDataPath", false, 10000)]
        private static void OpenPersistentDataPath() =>
            EditorUtility.RevealInFinder(Application.persistentDataPath);
    }
}