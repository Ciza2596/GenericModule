using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Audio;
using Object = UnityEngine.Object;

namespace CizaAudioModule
{
    public class AudioModule
    {
        //private variable
        private readonly IAudioModuleConfig _config;
        private readonly IAudioModuleAssetProvider _assetProvider;

        private Transform _poolRootTransform;
        private Dictionary<string, IAudioData> _audioDataMap;


        private readonly Dictionary<string, IAudio> _playingAudioMap = new();
        private readonly Dictionary<string, List<IAudio>> _unplayingAudiosMap = new();

        //public variable
        public AudioMixer AudioMixer { get; }
        public bool IsInitialized => _audioDataMap != null && _poolRootTransform != null;

        public string[] ClipDataIds { get; private set; }
        public string[] PrefabDataIds { get; private set; }
        public string[] AssetDataIds { get; private set; }


        //public method
        public AudioModule(AudioMixer audioMixer, IAudioModuleConfig config, IAudioModuleAssetProvider assetProvider)
        {
            _config = config;
            _assetProvider = assetProvider;
            Assert.IsNotNull(_config, $"[AudioModule::AudioModule] {nameof(IAudioModuleConfig)} is null.");
            Assert.IsNotNull(_assetProvider, $"[AudioModule::AudioModule] {nameof(assetProvider)} is null.");

            AudioMixer = audioMixer;
            Assert.IsNotNull(AudioMixer, $"[AudioModule::AudioModule] AudioMixer is null.");
        }


        public async UniTask Initialize(Dictionary<string, IAudioData> audioDataMap)
        {
            if (IsInitialized)
            {
                Debug.LogWarning("[AudioModule::Initialize] AudioModule is initialized.");
                return;
            }

            _audioDataMap = audioDataMap;
            Assert.IsNotNull(_audioDataMap, $"[AudioModule::Initialize] AudioMixer is null.");

            InitializeClipAndPrefabAndAssetDataIds();
            await LoadAssets();

            if (_poolRootTransform is null)
            {
                var poolRootGameObject = new GameObject(_config.PoolRootName);
                _poolRootTransform = poolRootGameObject.transform;
            }
        }

        public void Dispose()
        {
            if (!IsInitialized)
            {
                Debug.LogWarning("[AudioModule::Dispose] AudioModule is not initialized.");
                return;
            }

            StopAll();
            ReleaseUnplayingAudios();


            _audioDataMap = null;

            var poolRootGameObject = _poolRootTransform.gameObject;
            _poolRootTransform = null;

            DestroyOrImmediate(poolRootGameObject);

            _assetProvider.UnloadAssets(AssetDataIds);
        }


        public bool CheckIsPlaying(string audioId) =>
            _playingAudioMap.ContainsKey(audioId);


        public void SetAudioMixerVolume(float volume) =>
            SetAudioMixerFloat(_config.AudioMixerVolumeParameter, volume);

        public string Play(string clipDataId, Vector3 position = default, Transform parentTransform = null)
        {
            if (!IsInitialized)
            {
                Debug.LogWarning("[AudioModule::Play] AudioModule is not initialized.");
                return string.Empty;
            }

            var audioData = _config.GetAudioData(clipDataId);

            var audioId = Play(audioData, position, parentTransform);
            return audioId;
        }

        public string Play(IAudioData audioData, Vector3 position = default, Transform parentTransform = null, float volume = 1)
        {
            if (!IsInitialized)
            {
                Debug.LogWarning("[AudioModule::PlayByAudioData] AudioModule is not initialized.");
                return string.Empty;
            }

            var audioId = Guid.NewGuid().ToString();
            var clipDataId = audioData.ClipDataId;
            var prefabDataId = audioData.PrefabDataId;
            var spatialBlend = audioData.SpatialBlend;

            var audio = GetOrCreateAudio(prefabDataId);
            var audioClip = _assetProvider.GetAsset<AudioClip>(clipDataId);

            AddAudioToPlayingAudiosMap(audioId, audio);
            
            audio.Play(audioId, clipDataId, audioClip, spatialBlend, position, parentTransform, volume);
            audio.GameObject.name = clipDataId;

            return audioId;
        }

        public void Stop(string audioId)
        {
            if (!IsInitialized)
            {
                Debug.LogWarning("[AudioModule::Stop] AudioModule is not initialized.");
                return;
            }

            var audio = _playingAudioMap[audioId];
            audio.Stop();

            _playingAudioMap.Remove(audioId);
            AddAudioToUnplayingAudiosMap(audio);
        }

        public void StopAll()
        {
            if (!IsInitialized)
            {
                Debug.LogWarning("[AudioModule::StopAll] AudioModule is not initialized.");
                return;
            }

            var audioIds = _playingAudioMap.Keys.ToArray();
            foreach (var audioId in audioIds)
                Stop(audioId);
        }

        public void ReleaseUnplayingAudios()
        {
            if (!IsInitialized)
            {
                Debug.LogWarning("[AudioModule::ReleaseUnplayingAudios] AudioModule is not initialized.");
                return;
            }
            
            var prefabDataIds = _unplayingAudiosMap.Keys.ToArray();
            foreach (var prefabDataId in prefabDataIds)
                ReleaseUnplayingAudios(prefabDataId);
        }


        public void Resume(string audioId)
        {
            if (!IsInitialized)
            {
                Debug.LogWarning("[AudioModule::Resume] AudioModule is not initialized.");
                return;
            }

            if (!_playingAudioMap.ContainsKey(audioId))
            {
                Debug.LogWarning($"[AudioModule::Resume] Audio is not found by audioId: {audioId}.");
                return;
            }

            var usingAudio = _playingAudioMap[audioId];
            usingAudio.Resume();
        }

        public void Pause(string audioId)
        {
            if (!IsInitialized)
            {
                Debug.LogWarning("[AudioModule::Pause] AudioModule is not initialized.");
                return;
            }

            if (!_playingAudioMap.ContainsKey(audioId))
            {
                Debug.LogWarning($"[AudioModule::Pause] Audio is not found by audioId: {audioId}.");
                return;
            }

            var usingAudio = _playingAudioMap[audioId];
            usingAudio.Pause();
        }

        public void SetVolume(string audioId, float volume)
        {
            if (!IsInitialized)
            {
                Debug.LogWarning("[AudioModule::SetVolume] AudioModule is not initialized.");
                return;
            }

            if (!_playingAudioMap.ContainsKey(audioId))
            {
                Debug.LogWarning($"[AudioModule::SetVolume] Audio is not found by audioId: {audioId}.");
                return;
            }

            var usingAudio = _playingAudioMap[audioId];
            usingAudio.SetVolume(volume);
        }


        // private method

        private IAudio GetOrCreateAudio(string prefabDataId)
        {
            if (!_unplayingAudiosMap.ContainsKey(prefabDataId))
                _unplayingAudiosMap.Add(prefabDataId, new List<IAudio>());

            var unplayingAudios = _unplayingAudiosMap[prefabDataId];
            if (unplayingAudios.Count <= 0)
                CreateAudioAndAddToUnplayingAudiosMap(prefabDataId);

            var unplayingAudio = unplayingAudios.First();
            unplayingAudios.Remove(unplayingAudio);
            return unplayingAudio;
        }

        private void CreateAudioAndAddToUnplayingAudiosMap(string prefabDataId)
        {
            var audioPrefab = _assetProvider.GetAsset<GameObject>(prefabDataId);
            var audio = Object.Instantiate(audioPrefab).GetComponent<IAudio>();
            audio.Initialize(prefabDataId);

            AddAudioToUnplayingAudiosMap(audio);
        }

        private void AddAudioToUnplayingAudiosMap(IAudio audio)
        {
            audio.GameObject.name = audio.PrefabDataId;
            audio.GameObject.SetActive(false);

            var unplayingAudios = _unplayingAudiosMap[audio.PrefabDataId];
            unplayingAudios.Add(audio);
        }

        private void AddAudioToPlayingAudiosMap(string audioId, IAudio audio)
        {
            audio.GameObject.SetActive(true);
            _playingAudioMap.Add(audioId, audio);
        }


        private void SetAudioMixerFloat(string volumeParameter, float volume)
        {
            var linearToLogarithmicScale = GetLinearToLogarithmicScale(volume);
            AudioMixer.SetFloat(volumeParameter, linearToLogarithmicScale);
        }

        private float GetLinearToLogarithmicScale(float value) =>
            Mathf.Log(Mathf.Clamp(value, 0.001f, 1)) * 20.0f;

        private void InitializeClipAndPrefabAndAssetDataIds()
        {
            var audioDatas = _audioDataMap.Values.ToArray();
            var clipDataIds = new List<string>();
            var prefabDataIds = new List<string>();

            ForeachAudioDatas(audioDatas, audioData =>
            {
                clipDataIds.Add(audioData.ClipDataId);
                prefabDataIds.Add(audioData.PrefabDataId);
            });

            ClipDataIds = clipDataIds.ToArray();
            PrefabDataIds = prefabDataIds.ToArray();


            var assetDataIds = new List<string>(clipDataIds);
            assetDataIds.AddRange(prefabDataIds);
            AssetDataIds = assetDataIds.ToArray();
        }


        private void ForeachAudioDatas(IAudioData[] audioDatas, Action<IAudioData> action)
        {
            foreach (var audioData in audioDatas)
                action(audioData);
        }

        private async UniTask LoadAssets()
        {
            var uniTasks = new List<UniTask>();
            uniTasks.Add(_assetProvider.LoadAssets<AudioClip>(ClipDataIds));
            uniTasks.Add(_assetProvider.LoadAssets<GameObject>(PrefabDataIds));
            await UniTask.WhenAll(uniTasks);
        }

        private void ReleaseUnplayingAudios(string prefabDataId)
        {
            if (!_unplayingAudiosMap.ContainsKey(prefabDataId))
                return;

            var unplayingAudios = _unplayingAudiosMap[prefabDataId];
            var audios = unplayingAudios.ToArray();

            unplayingAudios.Clear();
            foreach (var audio in audios)
                DestroyOrImmediate(audio.GameObject);
        }
        
        private void DestroyOrImmediate(Object obj)
        {
            if (Application.isPlaying)
                Object.Destroy(obj);
            else
                Object.DestroyImmediate(obj);
        }
    }
}