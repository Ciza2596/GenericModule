namespace CizaAudioModule
{
	public interface IRestrictContinuousPlay
	{
		float Duration { get; }
		int MaxConsecutiveCount { get; }
	}
}