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

        private const string CREATE_AUDIO_PREFAB_EDITOR = "CreateAudioPrefabEditor.";

        private AudioMixer _audioMixer;


        private readonly string _volumeParameterKey = $"{CREATE_AUDIO_PREFAB_EDITOR}{nameof(VolumeParameter)}";

        private string VolumeParameter
        {
            get => PlayerPrefs.GetString(_volumeParameterKey);
            set
            {
                PlayerPrefs.SetString(_volumeParameterKey, value);
                PlayerPrefs.Save();
            }
        }


        private readonly string _isPlayOnAwakeKey = $"{CREATE_AUDIO_PREFAB_EDITOR}{nameof(IsPlayOnAwake)}";

        private bool IsPlayOnAwake
        {
            get => PlayerPrefs.GetInt(_isPlayOnAwakeKey) == 1;
            set
            {
                PlayerPrefs.SetInt(_isPlayOnAwakeKey, value ? 1 : 0);
                PlayerPrefs.Save();
            }
        }

        private readonly string _isLoopKey = $"{CREATE_AUDIO_PREFAB_EDITOR}{nameof(IsLoop)}";

        private bool IsLoop
        {
            get => PlayerPrefs.GetInt(_isLoopKey) == 1;
            set
            {
                PlayerPrefs.SetInt(_isLoopKey, value ? 1 : 0);
                PlayerPrefs.Save();
            }
        }


        private readonly string _spatialBlendKey = $"{CREATE_AUDIO_PREFAB_EDITOR}{nameof(SpatialBlend)}";

        private float SpatialBlend
        {
            get => PlayerPrefs.GetFloat(_spatialBlendKey);
            set
            {
                PlayerPrefs.SetFloat(_spatialBlendKey, value);
                PlayerPrefs.Save();
            }
        }


        private readonly string _audioPrefabFolderPathKey = $"{CREATE_AUDIO_PREFAB_EDITOR}{nameof(AudioPrefabFolderPath)}";

        private string AudioPrefabFolderPath
        {
            get
            {
                var value = PlayerPrefs.GetString(_audioPrefabFolderPathKey);
                return string.IsNullOrWhiteSpace(value) ? Application.dataPath : value;
            }

            set
            {
                PlayerPrefs.SetString(_audioPrefabFolderPathKey, value);
                PlayerPrefs.Save();
            }
        }


        private AudioClip _audioClip;


        private readonly string _audioClipFolderPathKey = $"{CREATE_AUDIO_PREFAB_EDITOR}{nameof(AudioClipFolderPath)}";

        private string AudioClipFolderPath
        {
            get
            {
                var value = PlayerPrefs.GetString(_audioClipFolderPathKey);
                return string.IsNullOrWhiteSpace(value) ? Application.dataPath : value;
            }

            set
            {
                PlayerPrefs.SetString(_audioClipFolderPathKey, value);
                PlayerPrefs.Save();
            }
        }


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
            VolumeParameter = EditorGUILayout.TextField("volumeParameter", VolumeParameter);

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            IsPlayOnAwake = EditorGUILayout.Toggle("isPlayOnAwake", IsPlayOnAwake);
            IsLoop = EditorGUILayout.Toggle("isLoop", IsLoop);
            SpatialBlend = EditorGUILayout.FloatField("spatialBlend", SpatialBlend);

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            AudioPrefabFolderPath = GetFolderPathAndOpenWindow("AudioPrefabFolderPath", AudioPrefabFolderPath);
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
            AudioClipFolderPath = GetFolderPathAndOpenWindow("AudioClipFolderPath", AudioClipFolderPath);

            EditorGUILayout.Space();

            if (GUILayout.Button("Create Audio Prefabs"))
            {
                var audioGuidList = AssetDatabase.FindAssets("t: AudioClip", new string[] { AudioClipFolderPath });

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

            audioSource.outputAudioMixerGroup = _audioMixer.FindMatchingGroups(VolumeParameter)[0];

            audioSource.playOnAwake = IsPlayOnAwake;
            audioSource.loop = IsLoop;
            audioSource.spatialBlend = SpatialBlend;

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
            var path = Path.Combine(AudioPrefabFolderPath, prefabName);
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