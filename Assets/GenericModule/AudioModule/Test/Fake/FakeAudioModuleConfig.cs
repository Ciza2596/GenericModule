using System.Collections.Generic;
using CizaAudioModule;

public class FakeAudioModuleConfig : IAudioModuleConfig
{
	private IReadOnlyDictionary<string, IAudioData> _audioDataMap;
	
    public string                                  AudioMixerVolumeParameter { get; }
    public string                                  PoolRootName              { get; }
    public string                                  PoolPrefix                { get; }
    public string                                  PoolSuffix                { get; }

    public IReadOnlyDictionary<string, IAudioData> CreateAudioDataMap() =>
	    _audioDataMap;
    
    public void SetAudioDataMap(Dictionary<string, IAudioData> audioDataMap) =>
	    _audioDataMap = audioDataMap;
    
}