using UnityEngine.Audio;

namespace CizaAudioModule
{
    public interface IAudioModuleConfig
    {
        string PoolRootName { get; }
        string PoolPrefix { get; }
        string PoolSuffix { get; }

        
        AudioMixer AudioMixer { get; }
        string MasterVolumeParameter { get; }

        string BgmVolumeParameter { get; }

        string SfxVolumeParameter { get; }

        string VoiceVolumeParameter { get; }
    }
}