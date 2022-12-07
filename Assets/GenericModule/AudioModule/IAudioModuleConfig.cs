using UnityEngine;
using UnityEngine.Audio;

namespace AudioModule
{
    public interface IAudioModuleConfig
    {
        AudioMixer AudioMixer { get; }

        string PoolName { get; }
        
        Transform PoolParentTransform { get; }

        string MasterVolumeParameter { get; }
        
        string BgmVolumeParameter { get; }

        string SfxVolumeParameter { get; }

        string VoiceVolumeParameter { get; }
    }
}

