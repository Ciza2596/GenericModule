using UnityEditor;
using UnityEngine;

namespace CizaAddressablesModule.Editor
{
	public class OpenAddressablesAaFolderMenu
	{
		public const string DefaultAddressablesOutputPath = "Library/com.unity.addressables/aa";

		[MenuItem("Tools/Ciza/OpenAddressablesAaFolder", false, 10000)]
		private static void OpenAddressablesAaFolder()
		{
			var fullPath = Application.dataPath.Replace("Assets","") + DefaultAddressablesOutputPath;

			if (System.IO.Directory.Exists(fullPath))
				EditorUtility.RevealInFinder(fullPath);

			else
				Debug.LogWarning($"The path: {fullPath} is not exist.");
		}
	}
}
