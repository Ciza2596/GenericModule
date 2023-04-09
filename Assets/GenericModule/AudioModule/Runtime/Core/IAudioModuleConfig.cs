
namespace CizaAudioModule
{
    public interface IAudioModuleConfig
    {
        string AudioMixerVolumeParameter { get; }

        string PoolRootName { get; }
        string PoolPrefix { get; }
        string PoolSuffix { get; }
    }
}
