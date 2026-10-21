namespace CizaAudioModule
{
	public interface IAudioChannelInfo
	{
		string MixerGroupPath { get; }
		string MixerVolumeParameter { get; }
		
		float MinMixerVolume { get; }
		float MaxMixerVolume { get; }
		float DefaultMixerVolume { get; }
	}
}