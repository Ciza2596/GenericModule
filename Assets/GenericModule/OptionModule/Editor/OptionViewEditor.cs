using UnityEditor;

namespace OptionModule.Editor
{
	[CustomEditor(typeof(OptionViewEditor))]
	[CanEditMultipleObjects]
	public class OptionViewEditor : UnityEditor.Editor
	{
		private SerializedProperty _collectionSettings;

		private SerializedProperty _isUseAnim;
		private SerializedProperty _animSettings;

		private void OnEnable()
		{
			_collectionSettings = serializedObject.FindProperty(nameof(_collectionSettings));

			_isUseAnim    = serializedObject.FindProperty(nameof(_isUseAnim));
			_animSettings = serializedObject.FindProperty(nameof(_animSettings));
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUILayout.PropertyField(_collectionSettings);
			EditorGUILayout.PropertyField(_isUseAnim);

			if (_isUseAnim.boolValue)
				EditorGUILayout.PropertyField(_animSettings);

			serializedObject.ApplyModifiedProperties();
		}
	}
}
