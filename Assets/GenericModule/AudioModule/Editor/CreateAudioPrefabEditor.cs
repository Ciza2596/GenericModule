using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

namespace AudioModule.Editor
{
    public class CreateAudioPrefabEditor : EditorWindow
    {
        //private variable
        private readonly string[] _toolbarTexts = { "CreateByFolder", "CreateByClip" };
        private int _toolbarIndex = 0;
        
        private AudioMixer _audioMixer;
        private string _volumeParameter;

        private bool _isPlayOnAwake;
        private bool _isLoop;
        private float _spatialBlend;

        private string _audioPrefabFolderPath = Application.dataPath;

        private AudioClip _audioClip;
        private string _audioClipFolderPath = Application.dataPath;


        //private method
        [MenuItem("Tools/CizaModule/CreateAudioPrefab")]
        private static void ShowWindow() =>
            GetWindow<CreateAudioPrefabEditor>("CreateAudioPrefab");

        private void OnGUI()
        {
            SettingsArea();

            EditorGUILayout.Space();

            ToolbarArea();

            EditorGUILayout.Space();

            switch (_toolbarIndex)
            {
                case 0:
                    CreateAudioPrefabsArea();
                    break;
                case 1:
                    CreateAudioPrefabArea();
                    break;
            }
        }

        private void SettingsArea()
        {
            _audioMixer =
                EditorGUILayout.ObjectField("AudioMixer", _audioMixer, typeof(AudioMixer), true) as AudioMixer;
            _volumeParameter = EditorGUILayout.TextField("volumeParameter", _volumeParameter);

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            _isPlayOnAwake = EditorGUILayout.Toggle("isPlayOnAwake", _isPlayOnAwake);
            _isLoop = EditorGUILayout.Toggle("isLoop", _isLoop);
            _spatialBlend = EditorGUILayout.FloatField("spatialBlend", _spatialBlend);

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            _audioPrefabFolderPath = GetFolderPathAndOpenWindow("AudioPrefabFolderPath", _audioPrefabFolderPath);
        }


        private void ToolbarArea()
        {
            GUILayout.BeginHorizontal();
            _toolbarIndex = GUILayout.Toolbar(_toolbarIndex, _toolbarTexts);
            GUILayout.EndHorizontal();
        }

        private void CreateAudioPrefabArea()
        {
            _audioClip = EditorGUILayout.ObjectField(_audioClip, typeof(AudioClip), true) as AudioClip;

            EditorGUILayout.Space();

            if (GUILayout.Button("Create Audio Prefab"))
                CreateAudioPrefab(_audioClip);
        }

        private void CreateAudioPrefabsArea()
        {
            _audioClipFolderPath = GetFolderPathAndOpenWindow("AudioClipFolderPath", _audioClipFolderPath);

            EditorGUILayout.Space();

            if (GUILayout.Button("Create Audio Prefabs"))
            {
                var audioGuidList = AssetDatabase.FindAssets("t: AudioClip", new string[] { _audioClipFolderPath });

                foreach (var guid in audioGuidList)
                {
                    var path = AssetDatabase.GUIDToAssetPath(guid);
                    var audioClip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
                    CreateAudioPrefab(audioClip);
                }
            }
        }

        private void CreateAudioPrefab(AudioClip audioClip)
        {
            var audioGameObject = CreateAudioGameObject(audioClip);
            CreatePrefab(audioGameObject);
        }


        private GameObject CreateAudioGameObject(AudioClip audioClip)
        {
            var audioGameObject = new GameObject(audioClip.name);
            var audioSource = audioGameObject.AddComponent<AudioSource>();
            audioSource.clip = audioClip;

            audioSource.outputAudioMixerGroup = _audioMixer.FindMatchingGroups(_volumeParameter)[0];

            audioSource.playOnAwake = _isPlayOnAwake;
            audioSource.loop = _isLoop;
            audioSource.spatialBlend = _spatialBlend;

            return audioGameObject;
        }


        private void CreatePrefab(GameObject audioGameObject)
        {
            var prefabName = audioGameObject.name;
            var audioPrefabPath = GetPrefabPath(prefabName);
            PrefabUtility.SaveAsPrefabAssetAndConnect(audioGameObject, audioPrefabPath, InteractionMode.UserAction);
        }


        private string GetPrefabPath(string prefabName)
        {
            prefabName += ".prefab";
            var path = Path.Combine(_audioPrefabFolderPath, prefabName);
            var localPath = AssetDatabase.GenerateUniqueAssetPath(path);
            return localPath;
        }

        private string GetFolderPathAndOpenWindow(string label, string originPath)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                var path = EditorGUILayout.TextField(label, originPath);
                if (GUILayout.Button("Select", EditorStyles.miniButton, GUILayout.Width(65)))
                    path = EditorUtility.OpenFolderPanel("Folder Path", "", "");

                return string.IsNullOrWhiteSpace(path) ? originPath : path;
            }
        }
    }
}