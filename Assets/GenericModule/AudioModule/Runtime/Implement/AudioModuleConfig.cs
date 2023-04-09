using UnityEngine;

namespace CizaAudioModule.Implement
{
    [CreateAssetMenu(fileName = "AudioModuleConfig", menuName = "Ciza/AudioModule/AudioModuleConfig")]
    public class AudioModuleConfig : ScriptableObject, IAudioModuleConfig
    {
        [SerializeField] private string _audioMixerVolumeParameter = "Master";
        [Space] [SerializeField] private string _poolRootName = "[AudioModulePoolRoot]";
        [SerializeField] private string _poolPrefix = "[";
        [SerializeField] private string _poolSuffix = "s]";


        public string AudioMixerVolumeParameter => _audioMixerVolumeParameter;

        public string PoolRootName => _poolRootName;
        public string PoolPrefix => _poolPrefix;
        public string PoolSuffix => _poolSuffix;
    }
}