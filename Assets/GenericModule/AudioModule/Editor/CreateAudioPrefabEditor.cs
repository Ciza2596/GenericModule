using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

namespace AudioModule.Editor
{
    public class CreateAudioPrefabEditor : EditorWindow
    {
        //private variable
        private AudioMixer _audioMixer;
        private string _volumeParameter;

        private bool _isPlayOnAwake;
        private bool _isLoop;
        private float _spatialBlend;

        private string _audioPrefabFolderPath;

        private AudioClip _audioClip;
        private string _audioClipFolderPath;

        private string[] _toolbarText = { "CreateByFolder", "CreateByClip" };
        private int _toolbarIndex = 0;


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
            _audioMixer = EditorGUILayout.ObjectField(_audioMixer, typeof(AudioMixer), true) as AudioMixer;
            _volumeParameter = EditorGUILayout.TextField("volumeParameter", _volumeParameter);

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            
            _isPlayOnAwake = EditorGUILayout.Toggle("isPlayOnAwake", _isPlayOnAwake);
            _isLoop = EditorGUILayout.Toggle("isLoop", _isLoop);
            _spatialBlend = EditorGUILayout.FloatField("spatialBlend", _spatialBlend);

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            _audioPrefabFolderPath = EditorGUILayout.TextField("AudioPrefabFolderPath", _audioPrefabFolderPath);
        }


        private void ToolbarArea()
        {
            GUILayout.BeginHorizontal();
            _toolbarIndex = GUILayout.Toolbar(_toolbarIndex, _toolbarText);
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
            _audioClipFolderPath = EditorGUILayout.TextField("AudioClipFolderPath", _audioClipFolderPath);

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
            var path = _audioPrefabFolderPath + "/" + prefabName + ".prefab";
            var localPath = AssetDatabase.GenerateUniqueAssetPath(path);
            return localPath;
        }
    }
}