namespace CizaAudioModule
{
	public interface IAudioInfo
	{
		string DataId { get; }

		string ClipAddress   { get; }
		string PrefabAddress { get; }
	}
}
