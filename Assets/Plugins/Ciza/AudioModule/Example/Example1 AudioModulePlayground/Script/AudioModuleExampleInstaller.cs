using System;
using System.Collections.Generic;
using System.Linq;
using CizaAudioModule.Implement;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace CizaAudioModule.Example1
{
    public class AudioModuleExampleInstaller : MonoBehaviour
    {
        // private variable

        [Space]
        [SerializeField]
        private AudioMixer _audioMixer;

        [SerializeField]
        private AudioModuleConfig _audioModuleConfig;

        [SerializeField]
        private AssetProvider _assetProvider;

        [SerializeField]
        private Vector3 _position;

        [Space]
        [SerializeField]
        private ComponentCollectionData _componentCollectionData;

        private AudioModule _audioModule;

        private readonly List<string> _audioIds = new();

        // unity callback
        private async void Awake()
        {
            _audioModule = new AudioModule(_audioModuleConfig, _assetProvider, _assetProvider, _audioMixer);
            _audioModule.Initialize();
            _audioModule.OnSpawn += m_OnPlay;
            _audioModule.OnStop += m_OnStop;

            var audioDataIds = _audioModule.AudioDataIds;
            foreach (var audioDataId in audioDataIds)
                await _audioModule.LoadAssetAsync(audioDataId, string.Empty);

            _componentCollectionData.AudioMixerVolumeSlider.onValueChanged.AddListener(SetAudioMixerVolume);

            _componentCollectionData.SetClipDataIds(audioDataIds);
            _componentCollectionData.PlayButton.onClick.AddListener(Play);
            UpdateAudioIds();

            _componentCollectionData.VolumeSlider.onValueChanged.AddListener(SetVolume);
            _componentCollectionData.PauseButton.onClick.AddListener(Pause);
            _componentCollectionData.ResumeButton.onClick.AddListener(Resume);
            _componentCollectionData.StopButton.onClick.AddListener(Stop);
            _componentCollectionData.StopAllButton.onClick.AddListener(StopAll);

            void m_OnStop(string callerId, string m_audioId, string m_audioDataId)
            {
                _audioIds.Remove(m_audioId);
                UpdateAudioIds();
            }

            void m_OnPlay(string callerId, string m_audioId, string m_audioDataId)
            {
                _audioIds.Add(m_audioId);
                UpdateAudioIds();
            }
        }

        private void Update()
        {
            if (_audioModule is { IsInitialized: true })
                _audioModule?.Tick(Time.deltaTime);
        }

        private void OnApplicationQuit()
        {
            if (_audioModule.IsInitialized)
                _audioModule.Release();
        }

        // private method
        private void SetAudioMixerVolume(float volume) =>
            _audioModule.SetVolume(volume);

        private async void Play()
        {
            var clipDataId = _componentCollectionData.ClipDataId;
            await _audioModule.PlayAsync(clipDataId, fadeTime: 0.25f, position: _position, isLoop: true);
        }

        private async void SetVolume(float volume)
        {
            var audioId = _componentCollectionData.AudioId;
            await _audioModule.ModifyAsync(audioId, volume, 0);
        }

        private void Pause()
        {
            var audioId = _componentCollectionData.AudioId;
            _audioModule.Pause(audioId);
        }

        private void Resume()
        {
            var audioId = _componentCollectionData.AudioId;
            _audioModule.Resume(audioId);
        }

        private async void Stop()
        {
            var audioId = _componentCollectionData.AudioId;
            await _audioModule.StopAsync(audioId);
            _audioIds.Remove(audioId);
            UpdateAudioIds();
        }

        private async void StopAll()
        {
            await _audioModule.StopAllAsync();
            _audioIds.Clear();
            UpdateAudioIds();
        }

        private void UpdateAudioIds()
        {
            var audioIds = _audioIds.ToArray().Reverse().ToArray();
            _componentCollectionData.SetAudioIds(audioIds);
        }

        [Serializable]
        private class ComponentCollectionData
        {
            [SerializeField]
            private Slider _audioMixerVolumeSlider;

            [Space]
            [SerializeField]
            private TMP_Dropdown _clipDataIdDropdown;

            [SerializeField]
            private Button _playButton;

            [Space]
            [SerializeField]
            private TMP_Dropdown _audioIdDropdown;

            [SerializeField]
            private Slider _volumeSlider;

            [SerializeField]
            private Button _pauseButton;

            [SerializeField]
            private Button _resumeButton;

            [SerializeField]
            private Button _stopButton;

            [SerializeField]
            private Button _stopAllButton;

            public Slider AudioMixerVolumeSlider => _audioMixerVolumeSlider;

            public string ClipDataId => _clipDataIdDropdown.captionText.text;
            public Button PlayButton => _playButton;

            public string AudioId => _audioIdDropdown.captionText.text;
            public Slider VolumeSlider => _volumeSlider;
            public Button PauseButton => _pauseButton;
            public Button ResumeButton => _resumeButton;
            public Button StopButton => _stopButton;
            public Button StopAllButton => _stopAllButton;

            public void SetClipDataIds(string[] clipDataIds)
            {
                _clipDataIdDropdown.ClearOptions();
                if (clipDataIds is not null && clipDataIds.Length > 0)
                    _clipDataIdDropdown.AddOptions(clipDataIds.ToList());
            }

            public void SetAudioIds(string[] audioIds)
            {
                _audioIdDropdown.ClearOptions();
                if (audioIds is not null && audioIds.Length > 0)
                    _audioIdDropdown.AddOptions(audioIds.ToList());
            }
        }
    }
}