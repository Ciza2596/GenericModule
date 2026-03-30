namespace CizaAudioModule
{
	public interface IAudioReadModel
	{
		string Id { get; }
		string DataId { get; }

		string CallerId { get; }
		bool IsRecord { get; }

		bool IsAutoDespawn { get; }
		string UserId { get; }

		string ClipAddress { get; }
		string PrefabAddress { get; }

		bool IsComplete { get; }

		bool IsLoop { get; }
		
		
		float Volume { get; }
		float CurrentVolume { get; }

		float Duration { get; }
		float Time { get; }
		bool IsPlaying { get; }
	}
}