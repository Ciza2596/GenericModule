using CizaAudioModule;
using UnityEngine.Audio;

namespace CizaAudioPlayerModule
{
	public interface IAudioPlayerModuleConfig
	{
		string RootName { get; }


		AudioMixer AudioMixer           { get; }
		string     MasterMixerGroupPath { get; }
		string     MasterMixerParameter { get; }
		float      DefaultMasterVolume  { get; }

		IAudioModuleConfig BgmModuleConfig   { get; }
		IAudioModuleConfig SfxModuleConfig    { get; }
		IAudioModuleConfig VoiceModuleConfig { get; }
	}
}
