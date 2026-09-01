namespace CizaAudioModule
{
	public interface IMultipleAudioChannelInfo
	{
		IAudioChannelInfo ExtraChannelInfo { get; }

		string[] ChannelDataIds { get; }
		bool TryGetChannelInfo(string dataId, out IAudioChannelInfo channelInfo);
	}
}