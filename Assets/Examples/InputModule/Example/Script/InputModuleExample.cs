using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CizaInputModule.Example
{
	public class InputModuleExample : MonoBehaviour
	{
		[SerializeField]
		private Settings _settings;

		[Space]
		[SerializeField]
		private PlayerInput _playerInput;


		private float _currentDuration;
		
		private void Update()
		{
			if(!_playerInput.TryGetDevices<Gamepad>(out var gamepads))
				return;
			
			var current = _playerInput.GetDevice<Gamepad>();
			if (current != null)
			{
				if (current.buttonSouth.wasPressedThisFrame)
				{
					Debug.Log("Shake");
					current.ResetHaptics();
					_currentDuration = _settings.Duration;
					current.SetMotorSpeeds(_settings.LowFrequency, _settings.HighFrequency);
				}
			
				if (_currentDuration > 0)
				{
					_currentDuration -= Time.deltaTime;
					if (_currentDuration <= 0)
					{
						Debug.Log("Stop Shake");
						current.ResetHaptics();
					}
				}
			}
		}

		private void Rumble(Gamepad[] gamepads)
		{
			
		}



		[Serializable]
		private class Settings
		{
			[SerializeField]
			private float _duration = 0.25f;

			[Space]
			[SerializeField]
			private float _lowFrequency = 0.1f;

			[SerializeField]
			private float _highFrequency = 0.1f;

			public float Duration => _duration;

			public float LowFrequency => _lowFrequency;

			public float HighFrequency => _highFrequency;
		}
	}
}
