using CizaInputModule.Implement;

namespace CizaInputModule
{
	public interface IRumbleInfo
	{
		public static IRumbleInfo CreateRumbleInfo(string dataId, float duration, float lowFrequency, float highFrequency) =>
			new RumbleInfo(dataId, duration, lowFrequency, highFrequency);

		string DataId { get; }

		float Duration { get; }

		float LowFrequency  { get; }
		float HighFrequency { get; }
	}
}
