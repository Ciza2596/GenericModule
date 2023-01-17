using System.Collections.Generic;
using AudioModule.Implement;
using Zenject;

namespace AudioModule.Example1
{
    public class AudioModuleExampleController : IInitializable
    {
        //private variable
        private AudioModule _audioModule;
        private AudioResourceDataOverview _audioResourceDataOverview;
        private ComponentCollectionData _componentCollectionData;

        private string _currentAudioId;
        private List<string> _audioIds = new List<string>();

        //public method
        public AudioModuleExampleController(AudioModule audioModule,
            AudioResourceDataOverview audioResourceDataOverview, ComponentCollectionData componentCollectionData)
        {
            _audioModule = audioModule;
            _audioResourceDataOverview = audioResourceDataOverview;
            _componentCollectionData = componentCollectionData;
        }

        public void Initialize()
        {
            var audioResourceDatas = _audioResourceDataOverview.GetAudioDatas;
            _audioModule.Initialize(audioResourceDatas);

            _componentCollectionData.PlayButton.onClick.AddListener(() =>
            {
                _currentAudioId = _audioModule.Play(_componentCollectionData.Key, _componentCollectionData.AudioLocalPosition,
                    _componentCollectionData.AudioParentTransform);
                _audioIds.Add(_currentAudioId);
            });
            
            _componentCollectionData.PauseButton.onClick.AddListener(() =>
            {
                _audioModule.Pause(_currentAudioId);
            });
            
            _componentCollectionData.ResumeButton.onClick.AddListener(() =>
            {
                _audioModule.Resume(_currentAudioId);
            });

            _componentCollectionData.StopButton.onClick.AddListener(() =>
            {
                _audioModule.Stop(_currentAudioId);
                _audioIds.Remove(_currentAudioId);
            });

            _componentCollectionData.StopAllButton.onClick.AddListener(() =>
            {
                var audioIds = _audioIds.ToArray();
                foreach (var audioId in audioIds)
                {
                    _audioModule.Stop(audioId);
                    _audioIds.Remove(audioId);
                }
            });
            
            _componentCollectionData.ReleasePoolButton.onClick.AddListener(() =>
            {
                _audioModule.ReleaseAllPool();
                _currentAudioId = string.Empty;
                _audioIds.Clear();
            });

            _componentCollectionData.MasterSlider.onValueChanged.AddListener(masterVolume =>
                _audioModule.SetMasterVolume(masterVolume));
            _componentCollectionData.BgmSlider.onValueChanged.AddListener(bgmVolume =>
                _audioModule.SetBgmVolume(bgmVolume));
            _componentCollectionData.SfxSlider.onValueChanged.AddListener(sfxVolume =>
                _audioModule.SetSfxVolume(sfxVolume));
            _componentCollectionData.VoiceSlider.onValueChanged.AddListener(voiceVolume =>
                _audioModule.SetVoiceVolume(voiceVolume));
        }
    }
}