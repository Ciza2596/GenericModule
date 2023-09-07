using UnityEditor;
using UnityEngine;

namespace CizaSaveLoadModule.Editor
{
	public class OpenPersistentFolderMenu
	{
		[MenuItem("Tools/Ciza/OpenPersistentFolder", false, 10000)]
		private static void OpenPersistentFolder() =>
			EditorUtility.RevealInFinder(Application.persistentDataPath);
	}
}
