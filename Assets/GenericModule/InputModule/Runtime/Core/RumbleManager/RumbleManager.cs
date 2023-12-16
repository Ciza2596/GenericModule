using System.Collections.Generic;
using System.Linq;
using CizaTimerModule;
using UnityEngine.InputSystem;

namespace CizaInputModule
{
	public class RumbleManager
	{
		private readonly IRumbleManagerConfig _rumbleManagerConfig;
		private readonly IRumbleInputs        _rumbleInputs;

		private readonly Dictionary<int, string> _timerIdMapByIndex = new Dictionary<int, string>();
		private readonly TimerModule             _timerModule       = new TimerModule();

		public RumbleManager(IRumbleManagerConfig rumbleManagerConfig, IRumbleInputs rumbleInputs)
		{
			_rumbleManagerConfig = rumbleManagerConfig;
			_rumbleInputs        = rumbleInputs;

			_timerModule.Initialize();

			_rumbleInputs.OnPlayerLeft += OnPlayerLeftImp;
		}

		public void Tick(float deltaTime) =>
			_timerModule.Tick(deltaTime);

		public void Rumble(int index, string dataId)
		{
			if (!CheckHasPlayer(index) || !_rumbleManagerConfig.TryGetRumbleInfo(dataId, out var rumbleInfo))
				return;

			_rumbleInputs.SetMotorSpeeds(index, rumbleInfo.LowFrequency, rumbleInfo.HighFrequency);

			AddTimer(index, rumbleInfo.Duration);
		}

		public void RumbleAll(string dataId)
		{
			for (var i = 0; i < _rumbleInputs.PlayerCount; i++)
				Rumble(i, dataId);
		}

		public void Stop(int index)
		{
			if (!_timerIdMapByIndex.ContainsKey(index))
				return;

			RemoveTimerIdAndResetHaptics(index);
		}

		public void StopAll()
		{
			foreach (var index in _timerIdMapByIndex.Keys.ToArray())
				Stop(index);
		}

		private bool CheckHasPlayer(int index) =>
			index >= 0 && index < _rumbleInputs.PlayerCount;

		private void AddTimer(int index, float duration)
		{
			RemoveTimer(index);

			var timerId = _timerModule.AddOnceTimer(duration, timerReadModel => { RemoveTimerIdAndResetHaptics(index); });
			_timerIdMapByIndex.Add(index, timerId);
		}

		private void RemoveTimer(int index)
		{
			if (!_timerIdMapByIndex.TryGetValue(index, out var timerId))
				return;

			_timerModule.RemoveTimer(timerId);
			_timerIdMapByIndex.Remove(index);
		}

		private void RemoveTimerIdAndResetHaptics(int index)
		{
			_rumbleInputs.ResetHaptics(index);
			_timerIdMapByIndex.Remove(index);
		}

		private void OnPlayerLeftImp(PlayerInput playerInput, InputModule inputModule) =>
			Stop(playerInput.playerIndex);
	}
}
