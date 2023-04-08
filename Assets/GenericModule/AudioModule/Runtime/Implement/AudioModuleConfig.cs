using UnityEngine;
using UnityEngine.Audio;

namespace CizaAudioModule.Implement
{
    [CreateAssetMenu(fileName = "AudioModuleConfig", menuName = "Ciza/AudioModule/AudioModuleConfig")]
    public class AudioModuleConfig : ScriptableObject, IAudioModuleConfig
    {
        [SerializeField] private string _poolRootName = "[AudioModulePoolRoot]";
        [SerializeField] private string _poolPrefix = "[";
        [SerializeField] private string _poolSuffix = "s]";

        [Space] 
        [SerializeField] private AudioMixer _audioMixer;
        [SerializeField] private string _masterVolumeParameter = "Master";
        [SerializeField] private string _bgmVolumeParameter = "Bgm";
        [SerializeField] private string _sfxVolumeParameter = "Sfx";
        [SerializeField] private string _voiceVolumeParameter = "Voice";
        

        public AudioMixer AudioMixer => _audioMixer;
        public string PoolRootName => _poolRootName;
        public string PoolPrefix => _poolPrefix;
        public string PoolSuffix => _poolSuffix;
        public IAudioData GetAudioData(string clipDataId) => throw new System.NotImplementedException();

        public string AudioMixerVolumeParameter => _masterVolumeParameter;
        public string BGMVolumeParameter => _bgmVolumeParameter;
        public string SFXVolumeParameter => _sfxVolumeParameter;
        public string VoiceVolumeParameter => _voiceVolumeParameter;
    }
}
