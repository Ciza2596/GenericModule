using UnityEngine.Audio;

namespace CizaAudioModule
{
	public interface IAudioPlayerConfig
	{
		string RootName { get; }
		bool IsDontDestroyOnLoad { get; }

		AudioMixer AudioMixer { get; }
		string MasterMixerGroupPath { get; }
		string MasterMixerParameter { get; }

		float MinMasterMixerVolume { get; }
		float MaxMasterMixerVolume { get; }
		float DefaultMasterMixerVolume { get; }

		IAudioModuleConfig BgmModuleConfig { get; }
		IAudioModuleConfig SfxModuleConfig { get; }
		IAudioModuleConfig VoiceModuleConfig { get; }
	}
}