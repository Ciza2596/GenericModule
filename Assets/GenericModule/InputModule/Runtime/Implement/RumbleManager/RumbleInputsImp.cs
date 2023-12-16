namespace CizaInputModule.Implement
{
	public class RumbleInputsImp : IRumbleInputs
	{
		private readonly InputModule _inputModule;

		public int PlayerCount => _inputModule.PlayerCount;

		public RumbleInputsImp(InputModule inputModule) =>
			_inputModule = inputModule;

		public void ResetHaptics(int index) =>
			_inputModule.ResetHaptics(index);

		public void SetMotorSpeeds(int index, float lowFrequency, float highFrequency) =>
			_inputModule.SetMotorSpeeds(index, lowFrequency, highFrequency);
	}
}
