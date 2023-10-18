namespace CizaAudioModule
{
	public interface IAudioReadModel
	{
		string Id     { get; }
		string DataId { get; }

		string ClipAddress   { get; }
		string PrefabAddress { get; }
		
		float  Volume   { get; }
		float  Duration { get; }
	}
}
