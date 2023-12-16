using System;
using UnityEngine.InputSystem;

namespace CizaInputModule.Implement
{
	public class RumbleInputsImp : IRumbleInputs
	{
		private readonly InputModule _inputModule;

		public event Action<PlayerInput, InputModule> OnPlayerLeft;

		public int PlayerCount => _inputModule.PlayerCount;

		public RumbleInputsImp(InputModule inputModule)
		{
			_inputModule = inputModule;

			_inputModule.OnPlayerLeft += OnPlayerLeftImp;
		}

		public void ResetHaptics(int index) =>
			_inputModule.ResetHaptics(index);

		public void SetMotorSpeeds(int index, float lowFrequency, float highFrequency) =>
			_inputModule.SetMotorSpeeds(index, lowFrequency, highFrequency);

		private void OnPlayerLeftImp(PlayerInput playerInput, InputModule inputModule) =>
			OnPlayerLeft?.Invoke(playerInput, inputModule);
	}
}
