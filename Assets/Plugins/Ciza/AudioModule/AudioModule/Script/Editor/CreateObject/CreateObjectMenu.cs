using System.IO;
using UnityEditor;
using UnityEngine;

namespace CizaAudioModule.Editor
{
	public static class CreateObjectMenu
	{
		public const string PROJECT_ID = "AudioModule";
		public const string ROOT_PATH = "Prefab";

		public const string AUDIO = "Audio";

		[MenuItem("GameObject/Ciza/AudioModule/Audio", false, -602)]
		public static void CreateLoadUi() =>
			CreateObject(AUDIO);

		private static void CreateObject(string dataId)
		{
			var prefab = Resources.Load<GameObject>(Path.Combine(PROJECT_ID, ROOT_PATH, dataId));
			var uiObject = Object.Instantiate(prefab, Selection.activeTransform);
			uiObject.name = dataId;
		}
	}
}