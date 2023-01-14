using UnityEngine.Audio;

namespace AudioModule
{
    public interface IAudioModuleConfig
    {
        AudioMixer AudioMixer { get; }

        string PoolRootName { get; }
        string PoolPrefix { get; }
        string PoolSuffix { get; }

        string MasterVolumeParameter { get; }

        string BgmVolumeParameter { get; }

        string SfxVolumeParameter { get; }

        string VoiceVolumeParameter { get; }
    }
}