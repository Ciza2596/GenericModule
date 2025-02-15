
namespace CizaAudioModule
{
	public interface IRestrictContinuousPlayEnabler: IEnabler<IRestrictContinuousPlay> { }


	public interface IRestrictContinuousPlay
	{
		float Duration { get; }
		int MaxConsecutiveCount { get; }
	}
}