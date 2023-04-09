using CizaAudioModule;

public class FakeAudioModuleConfig : IAudioModuleConfig
{
    private IAudioData _audioData;
    
    public string AudioMixerVolumeParameter { get; }
    public string PoolRootName { get; }
    public string PoolPrefix { get; }
    public string PoolSuffix { get; }

    public IAudioData GetAudioData(string clipDataId) =>
        _audioData;

    public void SetReturnAudioData(IAudioData audioData) =>
        _audioData = audioData;
}