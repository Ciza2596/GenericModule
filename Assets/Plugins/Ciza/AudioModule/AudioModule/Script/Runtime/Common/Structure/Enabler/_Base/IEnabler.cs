namespace CizaAudioModule
{
	public interface IEnabler<TValue>
	{
		bool IsEnable { get; }
		TValue Value { get; }

		bool TryGetValue(out TValue value);
	}
}