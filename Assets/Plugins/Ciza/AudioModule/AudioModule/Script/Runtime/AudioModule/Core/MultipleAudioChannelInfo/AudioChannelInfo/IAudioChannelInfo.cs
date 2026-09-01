namespace CizaAudioModule
{
	public interface IAudioChannelInfo
	{
		string AudioMixerGroupPath { get; }
		string AudioMixerVolumeParameter { get; }
		float DefaultVolume { get; }
	}
}