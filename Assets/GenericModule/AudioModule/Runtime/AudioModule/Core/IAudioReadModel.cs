namespace CizaAudioModule
{
	public interface IAudioReadModel
	{
		string Id     { get; }
		string DataId { get; }

		string CallerId { get; }

		string ClipAddress   { get; }
		string PrefabAddress { get; }

		bool IsComplete { get; }

		bool  IsLoop   { get; }
		float Volume   { get; }
		float Duration { get; }
	}
}
