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

        bool TryGetRestrictContinuousPlay(out IRestrictContinuousPlay restrictContinuousPlay);

        string DefaultPrefabAddress { get; }
        IReadOnlyDictionary<string, IAudioInfo> CreateAudioInfoMapDataId();
    }
}