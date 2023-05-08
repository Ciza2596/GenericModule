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
		[SerializeField]         private AudioDataOverview        _audioDataOverview;
		[Space] [SerializeField] private AudioMixer               _audioMixer;
		[SerializeField]         private AudioModuleConfig        _audioModuleConfig;
		[SerializeField]         private AudioModuleAssetProvider _audioModuleAssetProvider;
		[Space] [SerializeField] private Transform                _parentTransform;
		[SerializeField]         private bool                     _isLocalPosition;
		[SerializeField]         private Vector3                  _position;
		[Space] [SerializeField] private ComponentCollectionData  _componentCollectionData;

		private AudioModule _audioModule;

		private readonly List<string> _audioIds = new();

		// unity callback
		private async void Awake()
		{
			_audioModule = new AudioModule(_audioModuleConfig, _audioModuleAssetProvider, _audioMixer);
			_audioModule.Initialize();

			_componentCollectionData.AudioMixerVolumeSlider.onValueChanged.AddListener(SetAudioMixerVolume);

			var clipDataIds = _audioModule.ClipDataIds;
			_componentCollectionData.SetClipDataIds(clipDataIds);
			_componentCollectionData.PlayButton.onClick.AddListener(Play);

			_componentCollectionData.VolumeSlider.onValueChanged.AddListener(SetVolume);
			_componentCollectionData.PauseButton.onClick.AddListener(Pause);
			_componentCollectionData.ResumeButton.onClick.AddListener(Resume);
			_componentCollectionData.StopButton.onClick.AddListener(Stop);
			_componentCollectionData.StopAllButton.onClick.AddListener(StopAll);
		}

		private void OnApplicationQuit()
		{
			if (_audioModule.IsInitialized)
				_audioModule.Release();
		}

		// private method
		private void SetAudioMixerVolume(float volume) =>
			_audioModule.SetAudioMixerVolume(volume);

		private void Play()
		{
			var clipDataId = _componentCollectionData.ClipDataId;
			var isPlay     = _audioModule.TryPlay(clipDataId, out var audioId, position: _position, parentTransform: _parentTransform, isLocalPosition: _isLocalPosition);

			_audioIds.Add(audioId);
			UpdateAudioIds();
		}

		private void SetVolume(float volume)
		{
			var audioId = _componentCollectionData.AudioId;
			_audioModule.SetVolume(audioId, volume);
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

		private void Stop()
		{
			var audioId = _componentCollectionData.AudioId;
			_audioModule.Stop(audioId);
			_audioIds.Remove(audioId);
			UpdateAudioIds();
		}

		private void StopAll()
		{
			_audioModule.StopAll();
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
			[SerializeField]         private Slider       _audioMixerVolumeSlider;
			[Space] [SerializeField] private TMP_Dropdown _clipDataIdDropdown;
			[SerializeField]         private Button       _playButton;
			[Space] [SerializeField] private TMP_Dropdown _audioIdDropdown;
			[SerializeField]         private Slider       _volumeSlider;
			[SerializeField]         private Button       _pauseButton;
			[SerializeField]         private Button       _resumeButton;
			[SerializeField]         private Button       _stopButton;
			[SerializeField]         private Button       _stopAllButton;

			public Slider AudioMixerVolumeSlider => _audioMixerVolumeSlider;

			public string ClipDataId => _clipDataIdDropdown.captionText.text;
			public Button PlayButton => _playButton;

			public string AudioId       => _audioIdDropdown.captionText.text;
			public Slider VolumeSlider  => _volumeSlider;
			public Button PauseButton   => _pauseButton;
			public Button ResumeButton  => _resumeButton;
			public Button StopButton    => _stopButton;
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
