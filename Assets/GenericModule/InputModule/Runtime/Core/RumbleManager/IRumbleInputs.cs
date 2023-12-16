using System;
using UnityEngine.InputSystem;

namespace CizaInputModule
{
	public interface IRumbleInputs
	{
		event Action<PlayerInput, InputModule> OnPlayerLeft;

		int PlayerCount { get; }

		void ResetHaptics(int index);

		void SetMotorSpeeds(int index, float lowFrequency, float highFrequency);
	}
}
