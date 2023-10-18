
using System.Collections.Generic;

namespace CizaAudioModule
{
    public interface IAudioModuleConfig
    {
        string AudioMixerGroupPath { get; }

        string PoolRootName { get; }
        string PoolPrefix { get; }
        string PoolSuffix { get; }

        string DefaultPrefabAddress { get; }

        IReadOnlyDictionary<string, IAudioInfo> CreateAudioInfoMap();
    }
}
