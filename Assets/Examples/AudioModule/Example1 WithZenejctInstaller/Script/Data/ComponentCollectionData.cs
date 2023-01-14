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
        [SerializeField] private Vector3 _audioPosition;
        [SerializeField] private Transform _parentTransform;

        [Space] [SerializeField] private Button _playerButton;
        [SerializeField] private Button _stopButton;
        [SerializeField] private Button _stopAllButton;
        [SerializeField] private Button _releaseButton;

        public Slider MasterSlider => _masterSlider;
        public Slider BgmSlider => _bgmSlider;
        public Slider SfxSlider => _sfxSlider;
        public Slider VoiceSlider => _voiceSlider;


        public string Key => _inputField.text;
        public Transform ParentTransform => _parentTransform;
        public Vector3 AudioPosition => _audioPosition;
        
        
        public Button PlayerButton => _playerButton;
        public Button StopButton => _stopButton;
        public Button StopAllButton => _stopAllButton;
        public Button ReleaseButton => _releaseButton;
    }
}