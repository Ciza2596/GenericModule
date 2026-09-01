namespace CizaAudioModule
{
	public interface IMultipleAudioChannelInfo
	{
		IAudioChannelInfo ExtraChannelInfo { get; }

		bool TryGetChannelInfo(string dataId, out IAudioChannelInfo channelInfo);
	}
}