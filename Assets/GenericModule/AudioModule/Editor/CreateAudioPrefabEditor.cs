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

        
        private readonly string _audioMixerGuidKey = $"{CREATE_AUDIO_PREFAB_EDITOR}{nameof(AudioMixerGuid)}";
        private string AudioMixerGuid
        {
            get => PlayerPrefs.GetString(_audioMixerGuidKey);
            set
            {
                PlayerPrefs.SetString(_audioMixerGuidKey, value);
                PlayerPrefs.Save();
            }
        }
        
        
        private AudioMixer _audioMixer;
        private AudioMixer AudioMixer
        {
            get
            {
                if (_audioMixer is null)
                    _audioMixer = GetObject<AudioMixer>(AudioMixerGuid);

                return _audioMixer;
            }
            set
            {
                _audioMixer = value;
                var guid = _audioMixer is null
                    ? string.Empty
                    : AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(_audioMixer));
                AudioMixerGuid = guid;
            }
        }


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


        private readonly string _audioClipGuidKey = $"{CREATE_AUDIO_PREFAB_EDITOR}{nameof(AudioClipGuid)}";
        private string AudioClipGuid
        {
            get => PlayerPrefs.GetString(_audioClipGuidKey);
            set
            {
                PlayerPrefs.SetString(_audioClipGuidKey, value);
                PlayerPrefs.Save();
            }
        }
        
        
        private AudioClip _audioClip;
        private AudioClip AudioClip
        {
            get
            {
                if (_audioClip is null)
                    _audioClip = GetObject<AudioClip>(AudioClipGuid);

                return _audioClip;
            }
            set
            {
                _audioClip = value;
                var guid = _audioClip is null
                    ? string.Empty
                    : AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(_audioClip));
                AudioClipGuid = guid;
            }
        }


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
            AudioMixer =
                EditorGUILayout.ObjectField("AudioMixer", AudioMixer, typeof(AudioMixer), true) as AudioMixer;
            VolumeParameter = EditorGUILayout.TextField("volumeParameter", VolumeParameter);

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            IsPlayOnAwake = EditorGUILayout.Toggle("isPlayOnAwake", IsPlayOnAwake);
            IsLoop = EditorGUILayout.Toggle("isLoop", IsLoop);
            SpatialBlend = EditorGUILayout.Slider("spatialBlend", SpatialBlend,0,1);

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
            AudioClip = EditorGUILayout.ObjectField(AudioClip, typeof(AudioClip), true) as AudioClip;

            EditorGUILayout.Space();

            if (GUILayout.Button("Create Audio Prefab"))
                CreateAudioPrefab(AudioClip);
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

            audioSource.outputAudioMixerGroup = AudioMixer.FindMatchingGroups(VolumeParameter)[0];

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
            return path;
        }

        private string GetFolderPathAndOpenWindow(string label, string originPath)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                var path = EditorGUILayout.TextField(label, originPath);
                if (GUILayout.Button("Select", EditorStyles.miniButton, GUILayout.Width(65)))
                {
                    path = EditorUtility.OpenFolderPanel("Folder Path", originPath, "");
                    path = GetAssetPathWithoutDataPath(path);
                }

                return string.IsNullOrWhiteSpace(path) ? originPath : path;
            }
        }
        
        private T GetObject<T>(string guid) where T : Object
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var obj = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            return obj;
        }
        
        private string GetAssetPathWithoutDataPath(string path)
        {
            var dataPath = Application.dataPath;
            dataPath = dataPath.Replace("Assets", "");
            
            if(path.Contains(dataPath)) 
                path = path.Replace(dataPath, "");

            return path;
        }

        private string GetAssetPathWithDataPath(string path)
        {
            var dataPath = Application.dataPath;
            dataPath = dataPath.Replace("Assets", "");

            if (!path.Contains(dataPath))
                path = dataPath + path;

            return path;
        }
    }
}