using System.Collections.Generic;

namespace CizaAudioModule
{
	public interface IAudioModuleConfig
	{
		string PoolRootName { get; }

		string PoolPrefix { get; }
		string PoolSuffix { get; }

		string MixerGroupPath { get; }
		string MixerVolumeParameter { get; }
		
		float MinMixerVolume { get; }
		float MaxMixerVolume { get; }
		float DefaultMixerVolume { get; }


		bool TryGetExtraChannelInfo(out IAudioChannelInfo extraChannelInfo);

		string[] ChannelDataIds { get; }
		bool TryGetChannelInfo(string channelDataId, out IAudioChannelInfo channelInfo);


		bool TryGetRestrictContinuousPlay(out IRestrictContinuousPlay restrictContinuousPlay);

		string PrefabAddress { get; }
		IReadOnlyDictionary<string, IAudioInfo> CreateAudioInfoMapByDataId();
	}
}