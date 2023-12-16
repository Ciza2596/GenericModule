namespace CizaInputModule
{
	public interface IRumbleInputs
	{
		int PlayerCount { get; }

		void ResetHaptics(int index);

		void SetMotorSpeeds(int index, float lowFrequency, float highFrequency);
	}
}
