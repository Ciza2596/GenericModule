using System.Collections.Generic;
using CizaAudioModule;

// public class FakeAudioModuleConfig : IAudioModuleConfig
// {
// 	private IReadOnlyDictionary<string, IAudioInfo> _audioDataMap;
// 	
//     public string                                  AudioMixerGroupPath { get; }
//     public string                                  PoolRootName              { get; }
//     public string                                  PoolPrefix                { get; }
//     public string                                  PoolSuffix                { get; }
//
//     public IReadOnlyDictionary<string, IAudioInfo> CreateAudioInfoMap() =>
// 	    _audioDataMap;
//     
//     public void SetAudioDataMap(Dictionary<string, IAudioInfo> audioDataMap) =>
// 	    _audioDataMap = audioDataMap;
//     
// }