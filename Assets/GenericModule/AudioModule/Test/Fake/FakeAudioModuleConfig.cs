using CizaAudioModule;

public class FakeAudioModuleConfig : IAudioModuleConfig
{
    public string AudioMixerVolumeParameter { get; }
    public string PoolRootName { get; }
    public string PoolPrefix { get; }
    public string PoolSuffix { get; }
}