using System.Collections.Generic;
using UnityEngine.InputSystem;

namespace CizaInputModule
{
	public static class PlayerInputExtension
	{
		public static bool TryGetDevices<TInputDevice>(this PlayerInput playerInput, out TInputDevice[] devices) where TInputDevice : InputDevice
		{
			var inputDevices = new List<TInputDevice>();

			foreach (var device in playerInput.devices)
				if (device is TInputDevice inputDevice)
					inputDevices.Add(inputDevice);

			devices = inputDevices.ToArray();
			return devices.Length > 0;
		}
	}
}
