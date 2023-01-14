using UnityEngine;
using UnityEngine.Audio;

namespace AudioModule.Implement
{
    [CreateAssetMenu(fileName = "AudioModuleConfig", menuName = "AudioModule/AudioModuleConfig")]
    public class AudioModuleConfig : ScriptableObject, IAudioModuleConfig
    {
        [SerializeField]
        private AudioMixer _audioMixer;
        [SerializeField]
        private string _poolRootName = "[AudioModuleRootPool]";

        [Space]
        [SerializeField] private string _poolPrefix = "[";
        [SerializeField] private string _poolSuffix = "s]";

        [Space] [SerializeField] private string _masterVolumeParameter = "Master";
        [SerializeField] private string _bgmVolumeParameter = "Master/Bgm";
        [SerializeField] private string _sfxVolumeParameter = "Master/Sfx";
        [SerializeField] private string _voiceVolumeParameter = "Master/Voice";
        

        public AudioMixer AudioMixer => _audioMixer;
        public string PoolRootName => _poolRootName;
        public string PoolPrefix => _poolPrefix;
        public string PoolSuffix => _poolSuffix;

        public string MasterVolumeParameter => _masterVolumeParameter;
        public string BgmVolumeParameter => _bgmVolumeParameter;
        public string SfxVolumeParameter => _sfxVolumeParameter;
        public string VoiceVolumeParameter => _voiceVolumeParameter;
    }
}
