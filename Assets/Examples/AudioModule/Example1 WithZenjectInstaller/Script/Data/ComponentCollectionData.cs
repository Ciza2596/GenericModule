using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace AudioPlayerModule.Example1
{
    [Serializable]
    public class ComponentCollectionData
    {
        [SerializeField] private Slider _masterSlider;
        [SerializeField] private Slider _bgmSlider;
        [SerializeField] private Slider _sfxSlider;
        [SerializeField] private Slider _voiceSlider;

        [Space] [SerializeField] private TMP_InputField _inputField;
        [SerializeField] private Vector3 _audioLocalPosition;
        [SerializeField] private Transform _audioParentTransform;

        [Space] [SerializeField] private Button _playButton;
        [SerializeField] private Button _pauseButton;
        [SerializeField] private Button _resumeButton;
        [SerializeField] private Button _stopButton;
        [SerializeField] private Button _stopAllButton;
        [SerializeField] private Button _releasePoolButton;

        public Slider MasterSlider => _masterSlider;
        public Slider BgmSlider => _bgmSlider;
        public Slider SfxSlider => _sfxSlider;
        public Slider VoiceSlider => _voiceSlider;


        public string Key => _inputField.text;
        public Transform AudioParentTransform => _audioParentTransform;
        public Vector3 AudioLocalPosition => _audioLocalPosition;
        
        
        public Button PlayButton => _playButton;
        public Button PauseButton => _pauseButton;
        public Button ResumeButton => _resumeButton;
        public Button StopButton => _stopButton;
        public Button StopAllButton => _stopAllButton;
        public Button ReleasePoolButton => _releasePoolButton;
    }
}