using System.Collections.Generic;

namespace CizaAudioModule
{
	public interface IAudioModuleConfig
	{
		string PoolRootName { get; }

		string PoolPrefix { get; }
		string PoolSuffix { get; }

		string AudioMixerGroupPath { get; }
		string AudioMixerVolumeParameter { get; }
		float DefaultVolume { get; }

		bool HasMultipleChannels { get; }
		bool TryGetExtraChannelInfo(out IAudioChannelInfo channelInfo);
		bool TryGetChannelInfo(string dataId, out IAudioChannelInfo channelInfo);


		bool TryGetRestrictContinuousPlay(out IRestrictContinuousPlay restrictContinuousPlay);

		string PrefabAddress { get; }
		IReadOnlyDictionary<string, IAudioInfo> CreateAudioInfoMapByDataId();
	}
}