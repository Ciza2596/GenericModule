using CizaAudioModule;
using UnityEngine;

namespace CizaAudioPlayerModule.Implement
{
    [CreateAssetMenu(fileName = "AudioPlayerModuleConfig", menuName = "Ciza/AudioPlayerModule/AudioPlayerModuleConfig")]
    public class AudioPlayerModuleConfig : ScriptableObject, IAudioModuleConfig
    {
        [SerializeField] private string _audioMixerVolumeParameter = "Master";
        [Space] [SerializeField] private string _poolRootName = "[AudioPlayerModulePoolRoot]";
        [SerializeField] private string _poolPrefix = "[";
        [SerializeField] private string _poolSuffix = "s]";
        
        public string AudioMixerVolumeParameter => _audioMixerVolumeParameter;

        public string PoolRootName => _poolRootName;
        public string PoolPrefix => _poolPrefix;
        public string PoolSuffix => _poolSuffix;
    }
}