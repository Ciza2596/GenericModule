using CizaTextModule.Implement;
using UnityEditor;

namespace CizaTextModule.Editor
{
	[CustomEditor(typeof(TextModuleConfig))]
	[CanEditMultipleObjects]
	public class TextModuleConfigEditor : UnityEditor.Editor
	{
		private SerializedProperty _isCustomDefaultCategory;
		private SerializedProperty _customCustomDefaultCategory;

		private SerializedProperty _csvTextAsset;

		private SerializedProperty _isShowWarningLog;

		private void OnEnable()
		{
			_isCustomDefaultCategory     = serializedObject.FindProperty(nameof(_isCustomDefaultCategory));
			_customCustomDefaultCategory = serializedObject.FindProperty(nameof(_customCustomDefaultCategory));

			_csvTextAsset = serializedObject.FindProperty(nameof(_csvTextAsset));

			_isShowWarningLog = serializedObject.FindProperty(nameof(_isShowWarningLog));
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUILayout.PropertyField(_isCustomDefaultCategory);
			if (_isCustomDefaultCategory.boolValue)
				EditorGUILayout.PropertyField(_customCustomDefaultCategory);

			EditorGUILayout.PropertyField(_csvTextAsset);

			EditorGUILayout.PropertyField(_isShowWarningLog);

			serializedObject.ApplyModifiedProperties();
		}
	}
}
