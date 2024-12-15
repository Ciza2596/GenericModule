using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CizaTimerModule
{
	public class TimerModule
	{
		private Dictionary<string, Timer> _usingTimerMap;
		private List<Timer> _unusingTimers;

		public event Action<string> OnRemove;

		public bool IsInitialized => _usingTimerMap is not null && _unusingTimers is not null;

		public void Initialize()
		{
			if (IsInitialized)
			{
				Debug.LogWarning("[TimerModule::Initialize] TimerModule is initialized.");
				return;
			}

			_usingTimerMap = new();
			_unusingTimers = new();
		}

		public void Release()
		{
			if (!IsInitialized)
			{
				Debug.LogWarning("[TimerModule::Release] TimerModule is not initialized.");
				return;
			}

			_usingTimerMap.Clear();
			_usingTimerMap = null;

			_unusingTimers.Clear();
			_unusingTimers = null;
		}

		public void Tick(float deltaTime)
		{
			if (!IsInitialized)
			{
				Debug.LogWarning("[TimerModule::Tick] TimerModule is not initialized.");
				return;
			}

			foreach (var timer in _usingTimerMap.Values.ToArray())
			{
				timer.AddTime(deltaTime);
				timer.OnTick(deltaTime);

				if (timer.Time >= timer.Duration)
				{
					timer.OnComplete();

					timer.ResetTime();
					if (timer.IsOnce)
						RemoveTimer(timer.Id);
				}
			}
		}

		public bool TryGetTimerReadModel(string timerId, out ITimerReadModel timerReadModel)
		{
			timerReadModel = null;

			if (!IsInitialized)
			{
				Debug.LogWarning("[TimerModule::Tick] TimerModule is not initialized.");
				return false;
			}

			if (!_usingTimerMap.ContainsKey(timerId))
				return false;

			timerReadModel = _usingTimerMap[timerId];

			return true;
		}

		public string AddOnceTimer(float startValue, float targetValue, float duration, Action<ITimerReadModel, float> onTickValue, Action<ITimerReadModel> onComplete = null)
		{
			if (!IsInitialized)
			{
				Debug.LogWarning("[TimerModule::AddOnceTimer] TimerModule is not initialized.");
				return string.Empty;
			}

			var diffValueSpeed = (targetValue - startValue) / duration;
			var value = startValue;
			var id = AddOnceTimer(duration, onComplete, (timerReadModel, deltaTime) =>
			{
				var diffValueSpeedDeltaTime = diffValueSpeed * deltaTime;
				value += diffValueSpeedDeltaTime;
				if (Mathf.Approximately(value, targetValue))
					value = targetValue;
				onTickValue(timerReadModel, value);
			});

			return id;
		}

		public string AddOnceTimer(float duration, Action<ITimerReadModel> onComplete, Action<ITimerReadModel, float> onTick = null)
		{
			if (!IsInitialized)
			{
				Debug.LogWarning("[TimerModule::AddOnceTimer] TimerModule is not initialized.");
				return string.Empty;
			}

			return AddTimerToUsingTimerMap(true, duration, onComplete, onTick);
		}

		public string AddLoopTimer(float duration, Action<ITimerReadModel> onComplete, Action<ITimerReadModel, float> onTick = null)
		{
			if (!IsInitialized)
			{
				Debug.LogWarning("[TimerModule::AddLoopTimer] TimerModule is not initialized.");
				return string.Empty;
			}

			return AddTimerToUsingTimerMap(false, duration, onComplete, onTick);
		}

		public void RemoveTimer(string timerId)
		{
			if (!IsInitialized)
			{
				Debug.LogWarning("[TimerModule::RemoveTimer] TimerModule is not initialized.");
				return;
			}

			if (!_usingTimerMap.ContainsKey(timerId))
			{
				Debug.LogWarning($"[TimerModule::RemoveTimer] Timer is not found by timerId: {timerId}.");
				return;
			}

			RemoveTimerFromUsingTimerMap(timerId);
			OnRemove?.Invoke(timerId);
		}

		// private method
		private string AddTimerToUsingTimerMap(bool isOnce, float duration, Action<ITimerReadModel> onComplete, Action<ITimerReadModel, float> onTick = null)
		{
			var timerId = Guid.NewGuid().ToString();

			var timer = GetTimerFromUnusingTimer();
			timer.Initialize(timerId, isOnce, duration, onComplete, onTick);

			_usingTimerMap.Add(timerId, timer);

			return timerId;
		}

		private void RemoveTimerFromUsingTimerMap(string timerId)
		{
			var timer = _usingTimerMap[timerId];
			_usingTimerMap.Remove(timerId);

			_unusingTimers.Add(timer);
		}

		private Timer GetTimerFromUnusingTimer()
		{
			if (_unusingTimers.Count <= 0)
				_unusingTimers.Add(new Timer());

			var timer = _unusingTimers.First();
			_unusingTimers.Remove(timer);

			return timer;
		}
	}
}