using System.Collections.Generic;
using AudioModule.Implement;
using Zenject;

namespace AudioPlayerModule.Example1
{
    public class AudioPlayerModuleExampleController : IInitializable
    {
        //private variable
        private AudioPlayerModule _audioPlayerModule;
        private AudioResourceDataOverview _audioResourceDataOverview;
        private ComponentCollectionData _componentCollectionData;

        private string _currentAudioChannel;
        private string _currentAudioId;
        private Dictionary<string, List<string>> _audioChannelIdsMaps = new Dictionary<string, List<string>>();

        //public method
        public AudioPlayerModuleExampleController(AudioPlayerModule audioPlayerModule,
            AudioResourceDataOverview audioResourceDataOverview, ComponentCollectionData componentCollectionData)
        {
            _audioPlayerModule = audioPlayerModule;
            _audioResourceDataOverview = audioResourceDataOverview;
            _componentCollectionData = componentCollectionData;
        }

        public void Initialize()
        {
            var audioResourceDatas = _audioResourceDataOverview.GetAudioDatas;
            _audioPlayerModule.Initialize(audioResourceDatas);

            _componentCollectionData.PlayButton.onClick.AddListener(() =>
            {
                _currentAudioChannel = _componentCollectionData.Channel;
                _currentAudioId = _audioPlayerModule.Play(_currentAudioChannel,
                    _componentCollectionData.Key, _componentCollectionData.FadeTime,
                    _componentCollectionData.AudioLocalPosition,
                    _componentCollectionData.AudioParentTransform);

                if (!_audioChannelIdsMaps.ContainsKey(_currentAudioChannel))
                    _audioChannelIdsMaps.Add(_currentAudioChannel, new List<string>());

                var ids = _audioChannelIdsMaps[_currentAudioChannel];
                ids.Add(_currentAudioId);
            });

            _componentCollectionData.PlayAndAutoStopButton.onClick.AddListener(() =>
            {
                _audioPlayerModule.PlayAndAutoStop(_componentCollectionData.Channel, _componentCollectionData.Key,
                    _componentCollectionData.FadeTime,
                    _componentCollectionData.AudioLocalPosition,
                    _componentCollectionData.AudioParentTransform);
            });

            
            _componentCollectionData.PauseButton.onClick.AddListener(() =>
            {
                _audioPlayerModule.Pause(_currentAudioId, _componentCollectionData.FadeTime);
            });
            
            _componentCollectionData.PauseByChannelButton.onClick.AddListener(() =>
            {
                _audioPlayerModule.PauseByChannel(_componentCollectionData.Channel, _componentCollectionData.FadeTime);
            });
            

            _componentCollectionData.ResumeButton.onClick.AddListener(() =>
            {
                _audioPlayerModule.Resume(_currentAudioId, _componentCollectionData.FadeTime);
            });
            
            _componentCollectionData.ResumeByChannelButton.onClick.AddListener(() =>
            {
                _audioPlayerModule.ResumeByChannel(_componentCollectionData.Channel, _componentCollectionData.FadeTime);
            });
            
            
            _componentCollectionData.StopButton.onClick.AddListener(() =>
            {
                _audioPlayerModule.Stop(_currentAudioId);

                var ids = _audioChannelIdsMaps[_currentAudioChannel];
                ids.Remove(_currentAudioId);
                _currentAudioId = string.Empty;
            });

            _componentCollectionData.StopChannelButton.onClick.AddListener(() =>
            {
                var channel = _componentCollectionData.Channel;
                _audioPlayerModule.StopByChannel(channel);
                _audioChannelIdsMaps.Remove(channel);
            });

            _componentCollectionData.StopAllButton.onClick.AddListener(() =>
            {
                _audioPlayerModule.StopAll();

                _audioChannelIdsMaps.Clear();
                _currentAudioChannel = string.Empty;
                _currentAudioId = string.Empty;
            });

            _componentCollectionData.ReleasePoolButton.onClick.AddListener(() =>
            {
                _audioPlayerModule.ReleaseAllPool();

                _audioChannelIdsMaps.Clear();
                _currentAudioChannel = string.Empty;
                _currentAudioId = string.Empty;
            });


            _componentCollectionData.ChangeVolumeSlider.onValueChanged.AddListener(value =>
            {
                _audioPlayerModule.ChangeVolume(_currentAudioId, value, _componentCollectionData.FadeTime);
            });
            
            _componentCollectionData.ChangeVolumeByChannelSlider.onValueChanged.AddListener(value =>
            {
                _audioPlayerModule.ChangeVolumeByChannel(_currentAudioChannel, value, _componentCollectionData.FadeTime);
            });
        }
    }
}