using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace CizaAudioModule.Example1
{
    [Serializable]
    public class ComponentCollectionData
    {
        [Space] [SerializeField] private TMP_InputField _keyInputField;
        [SerializeField] private Vector3 _audioLocalPosition;
        [SerializeField] private Transform _audioParentTransform;

        [Space] [SerializeField] private Button _playButton;
        [SerializeField] private Button _pauseButton;
        [SerializeField] private Button _resumeButton;
        [SerializeField] private Button _stopButton;
        [SerializeField] private Button _stopAllButton;
        [SerializeField] private Button _releasePoolByKeyButton;
        [SerializeField] private Button _releaseAllPoolsButton;
        
        [Space]
        [SerializeField] private Slider _masterSlider;
        [SerializeField] private Slider _bgmSlider;
        [SerializeField] private Slider _sfxSlider;
        [SerializeField] private Slider _voiceSlider;


        public string Key => _keyInputField.text;
        public Transform AudioParentTransform => _audioParentTransform;
        public Vector3 AudioLocalPosition => _audioLocalPosition;
        
        
        public Button PlayButton => _playButton;
        public Button PauseButton => _pauseButton;
        public Button ResumeButton => _resumeButton;
        public Button StopButton => _stopButton;
        public Button StopAllButton => _stopAllButton;
        public Button ReleasePoolByKeyButton => _releasePoolByKeyButton;
        public Button ReleaseAllPoolsButton => _releaseAllPoolsButton;
        
        
        public Slider MasterSlider => _masterSlider;
        public Slider BgmSlider => _bgmSlider;
        public Slider SfxSlider => _sfxSlider;
        public Slider VoiceSlider => _voiceSlider;
    }
}