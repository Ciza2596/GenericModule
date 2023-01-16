using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace AudioPlayerModule.Example1
{
    [Serializable]
    public class ComponentCollectionData
    {
        [Space] [SerializeField] private TMP_InputField _channelInputField;
        [SerializeField] private TMP_InputField _keyInputField;
        [SerializeField] private TMP_InputField _fadeTimeInputField;
        [SerializeField] private Vector3 _audioLocalPosition;
        [SerializeField] private Transform _audioParentTransform;

        [Space] [SerializeField] private Button _playButton;
        [SerializeField] private Button _playAndAutoStopButton;
        [SerializeField] private Button _stopButton;
        [SerializeField] private Button _stopChannelButton;
        [SerializeField] private Button _stopAllButton;
        [SerializeField] private Button _releasePoolButton;
        
        [Space]
        [SerializeField] private Slider _changeVolumeSlider;


        public string Channel => _channelInputField.text;
        public string Key => _keyInputField.text;
        public float FadeTime => float.Parse(_fadeTimeInputField.text);
        public Transform AudioParentTransform => _audioParentTransform;
        public Vector3 AudioLocalPosition => _audioLocalPosition;


        public Button PlayButton => _playButton;
        public Button PlayAndAutoStopButton => _playAndAutoStopButton;
        public Button StopButton => _stopButton;
        public Button StopChannelButton => _stopChannelButton;
        public Button StopAllButton => _stopAllButton;
        public Button ReleasePoolButton => _releasePoolButton;
        
        
        public Slider ChangeVolumeSlider => _changeVolumeSlider;
    }
}