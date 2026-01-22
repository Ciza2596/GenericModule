using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CizaTimerModule
{
	public class TimerModule
	{
		protected Dictionary<string, Timer> _usingTimerMap;
		protected List<Timer> _unusingTimers;

		public event Action<string> OnRemove;

		public virtual bool IsInitialized => _usingTimerMap is not null && _unusingTimers is not null;

		public virtual void Initialize()
		{
			if (IsInitialized)
			{
				Debug.LogWarning("[TimerModule::Initialize] TimerModule is initialized.");
				return;
			}

			_usingTimerMap = new();
			_unusingTimers = new();
		}

		public virtual void Release()
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

		public virtual void Tick(float deltaTime)
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

				if (timer.IsPlayed)
				{
					timer.OnComplete();

					timer.ResetTime();
					if (timer.IsOnce)
						RemoveTimer(timer.Id);
				}
			}
		}

		public bool CheckHasTimer(string timerId) =>
			TryGetTimerReadModel(timerId, out _);

		public virtual bool TryGetTimerReadModel(string timerId, out ITimerReadModel timerReadModel)
		{
			timerReadModel = null;

			if (!IsInitialized)
			{
				Debug.LogWarning("[TimerModule::TryGetTimerReadModel] TimerModule is not initialized.");
				return false;
			}

			if (!_usingTimerMap.ContainsKey(timerId))
				return false;

			timerReadModel = _usingTimerMap[timerId];

			return true;
		}

		public virtual string AddOnceTimer(float startValue, float targetValue, float duration, Action<ITimerReadModel, float> onTickValue, Action<ITimerReadModel> onComplete = null) =>
			AddOnceTimer(false, string.Empty, startValue, targetValue, duration, onTickValue, onComplete);

		public virtual string AddOnceTimer(string timerId, float startValue, float targetValue, float duration, Action<ITimerReadModel, float> onTickValue, Action<ITimerReadModel> onComplete = null) =>
			AddOnceTimer(true, timerId, startValue, targetValue, duration, onTickValue, onComplete);

		public virtual string AddOnceTimer(float duration, Action<ITimerReadModel> onComplete, Action<ITimerReadModel, float> onTick = null) =>
			AddOnceTimer(false, string.Empty, duration, onComplete, onTick);

		public virtual string AddOnceTimer(string timerId, float duration, Action<ITimerReadModel> onComplete, Action<ITimerReadModel, float> onTick = null) =>
			AddOnceTimer(true, timerId, duration, onComplete, onTick);

		public virtual string AddLoopTimer(float duration, Action<ITimerReadModel> onComplete, Action<ITimerReadModel, float> onTick = null) =>
			AddLoopTimer(false, string.Empty, duration, onComplete, onTick);

		public virtual string AddLoopTimer(string timerId, float duration, Action<ITimerReadModel> onComplete, Action<ITimerReadModel, float> onTick = null) =>
			AddLoopTimer(true, timerId, duration, onComplete, onTick);


		public virtual void RemoveTimer(string timerId)
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
		protected virtual string AddOnceTimer(bool isCustomId, string timerId, float startValue, float targetValue, float duration, Action<ITimerReadModel, float> onTickValue, Action<ITimerReadModel> onComplete)
		{
			if (!IsInitialized)
			{
				Debug.LogWarning("[TimerModule::AddOnceTimer] TimerModule is not initialized.");
				return string.Empty;
			}

			var diffValueSpeed = (targetValue - startValue) / duration;
			var value = startValue;
			var id = AddOnceTimer(isCustomId, timerId, duration, onComplete, (timerReadModel, deltaTime) =>
			{
				var diffValueSpeedDeltaTime = diffValueSpeed * deltaTime;
				value += diffValueSpeedDeltaTime;
				if (timerReadModel.IsPlayed)
					value = targetValue;
				onTickValue(timerReadModel, value);
			});

			return id;
		}


		protected virtual string AddOnceTimer(bool isCustomId, string timerId, float duration, Action<ITimerReadModel> onComplete, Action<ITimerReadModel, float> onTick)
		{
			if (!IsInitialized)
			{
				Debug.LogWarning("[TimerModule::AddOnceTimer] TimerModule is not initialized.");
				return string.Empty;
			}

			return AddTimerToUsingTimerMap(isCustomId, timerId, true, duration, onComplete, onTick);
		}

		protected virtual string AddLoopTimer(bool isCustomId, string timerId, float duration, Action<ITimerReadModel> onComplete, Action<ITimerReadModel, float> onTick)
		{
			if (!IsInitialized)
			{
				Debug.LogWarning("[TimerModule::AddLoopTimer] TimerModule is not initialized.");
				return string.Empty;
			}

			return AddTimerToUsingTimerMap(isCustomId, timerId, false, duration, onComplete, onTick);
		}


		protected virtual string AddTimerToUsingTimerMap(bool isCustomId, string customId, bool isOnce, float duration, Action<ITimerReadModel> onComplete, Action<ITimerReadModel, float> onTick = null)
		{
			var timerId = isCustomId ? customId : Guid.NewGuid().ToString();

			var timer = GetTimerFromUnusingTimer();
			timer.Initialize(timerId, isOnce, duration, onComplete, onTick);

			_usingTimerMap.Add(timerId, timer);

			return timerId;
		}

		protected virtual void RemoveTimerFromUsingTimerMap(string timerId)
		{
			var timer = _usingTimerMap[timerId];
			_usingTimerMap.Remove(timerId);
			timer.ResetTime();

			_unusingTimers.Add(timer);
		}

		protected virtual Timer GetTimerFromUnusingTimer()
		{
			if (_unusingTimers.Count <= 0)
				_unusingTimers.Add(new Timer());

			var timer = _unusingTimers.First();
			_unusingTimers.Remove(timer);

			return timer;
		}
	}
}