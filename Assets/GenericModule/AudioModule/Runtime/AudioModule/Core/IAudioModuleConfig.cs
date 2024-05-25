using System.Collections.Generic;

namespace CizaAudioModule
{
    public interface IAudioModuleConfig
    {
        string PoolRootName { get; }

        string PoolPrefix { get; }
        string PoolSuffix { get; }

        string AudioMixerGroupPath { get; }
        string AudioMixerParameter { get; }
        float DefaultVolume { get; }

        IRestrictContinuousPlay RestrictContinuousPlay { get; }

        string DefaultPrefabAddress { get; }
        IReadOnlyDictionary<string, IAudioInfo> CreateAudioInfoMapDataId();
    }
}