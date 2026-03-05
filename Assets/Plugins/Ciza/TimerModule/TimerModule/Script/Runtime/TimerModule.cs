using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Scripting;

namespace CizaTimerModule
{
	public class TimerModule
	{
		// VARIABLE: -----------------------------------------------------------------------------

		protected Dictionary<string, Timer> _usingTimerMap;
		protected List<Timer> _unusingTimers;

		// EVENT: ---------------------------------------------------------------------------------

		public event Action<string> OnRemove;

		// PUBLIC VARIABLE: ---------------------------------------------------------------------

		public virtual bool IsInitialized => _usingTimerMap is not null && _unusingTimers is not null;

		public virtual bool CheckHasTimer(string timerId) =>
			TryGetTimerReadModel(timerId, out _);

		public virtual bool TryGetTimerReadModel(string timerId, out ITimerReadModel timerReadModel)
		{
			if (!IsInitialized)
			{
				timerReadModel = null;
				Debug.LogWarning("[TimerModule::TryGetTimerReadModel] TimerModule is not initialized.");
				return false;
			}

			if (!_usingTimerMap.TryGetValue(timerId, out var timer))
			{
				timerReadModel = null;
				return false;
			}

			timerReadModel = timer;
			return true;
		}

		// CONSTRUCTOR: ------------------------------------------------------------------------

		[Preserve]
		public TimerModule() { }

		// LIFECYCLE METHOD: ------------------------------------------------------------------

		public virtual void Initialize()
		{
			if (IsInitialized)
			{
				Debug.LogWarning("[TimerModule::Initialize] TimerModule is initialized.");
				return;
			}

			_usingTimerMap = new Dictionary<string, Timer>();
			_unusingTimers = new List<Timer>();
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

		// PUBLIC METHOD: ----------------------------------------------------------------------

		public virtual string AddOnceTimer(float startValue, float targetValue, float duration, Action<ITimerReadModel, float> onTickValue, Action<ITimerReadModel> onComplete = null, params object[] args) =>
			AddOnceTimer(startValue, targetValue, duration, 0, onTickValue, onComplete, args);

		public virtual string AddOnceTimer(float startValue, float targetValue, float duration, float time, Action<ITimerReadModel, float> onTickValue, Action<ITimerReadModel> onComplete = null, params object[] args) =>
			AddOnceTimer(false, string.Empty, startValue, targetValue, duration, time, onTickValue, onComplete, args);


		public virtual string AddOnceTimer(string timerId, float startValue, float targetValue, float duration, Action<ITimerReadModel, float> onTickValue, Action<ITimerReadModel> onComplete = null, params object[] args) =>
			AddOnceTimer(timerId, startValue, targetValue, duration, 0, onTickValue, onComplete, args);

		public virtual string AddOnceTimer(string timerId, float startValue, float targetValue, float duration, float time, Action<ITimerReadModel, float> onTickValue, Action<ITimerReadModel> onComplete = null, params object[] args) =>
			AddOnceTimer(true, timerId, startValue, targetValue, duration, time, onTickValue, onComplete, args);


		public virtual string AddOnceTimer(float duration, Action<ITimerReadModel> onComplete, Action<ITimerReadModel, float> onTick = null, params object[] args) =>
			AddOnceTimer(duration, 0, onComplete, onTick, args);

		public virtual string AddOnceTimer(float duration, float time, Action<ITimerReadModel> onComplete, Action<ITimerReadModel, float> onTick = null, params object[] args) =>
			AddOnceTimer(false, string.Empty, duration, time, onComplete, onTick, args);


		public virtual string AddOnceTimer(string timerId, float duration, Action<ITimerReadModel> onComplete, Action<ITimerReadModel, float> onTick = null, params object[] args) =>
			AddOnceTimer(timerId, duration, 0, onComplete, onTick, args);

		public virtual string AddOnceTimer(string timerId, float duration, float time, Action<ITimerReadModel> onComplete, Action<ITimerReadModel, float> onTick = null, params object[] args) =>
			AddOnceTimer(true, timerId, duration, time, onComplete, onTick, args);


		public virtual string AddLoopTimer(float duration, Action<ITimerReadModel> onComplete, Action<ITimerReadModel, float> onTick = null, params object[] args) =>
			AddLoopTimer(duration, 0, onComplete, onTick, args);

		public virtual string AddLoopTimer(float duration, float time, Action<ITimerReadModel> onComplete, Action<ITimerReadModel, float> onTick = null, params object[] args) =>
			AddLoopTimer(false, string.Empty, duration, time, onComplete, onTick, args);


		public virtual string AddLoopTimer(string timerId, float duration, Action<ITimerReadModel> onComplete, Action<ITimerReadModel, float> onTick = null, params object[] args) =>
			AddLoopTimer(timerId, duration, 0, onComplete, onTick, args);

		public virtual string AddLoopTimer(string timerId, float duration, float time, Action<ITimerReadModel> onComplete, Action<ITimerReadModel, float> onTick = null, params object[] args) =>
			AddLoopTimer(true, timerId, duration, time, onComplete, onTick, args);

		public virtual void RemoveTimer(string timerId)
		{
			if (!IsInitialized)
			{
				Debug.LogWarning("[TimerModule::RemoveTimer] TimerModule is not initialized.");
				return;
			}

			if (!_usingTimerMap.ContainsKey(timerId))
				return;

			RemoveTimerFromUsingTimerMap(timerId);
			OnRemove?.Invoke(timerId);
		}


		// PROTECT METHOD: --------------------------------------------------------------------

		protected virtual string AddOnceTimer(bool isCustomId, string timerId, float startValue, float targetValue, float duration, float time, Action<ITimerReadModel, float> onTickValue, Action<ITimerReadModel> onComplete, params object[] args)
		{
			if (!IsInitialized)
			{
				Debug.LogWarning("[TimerModule::AddOnceTimer] TimerModule is not initialized.");
				return string.Empty;
			}

			var diffValue = targetValue - startValue;
			var id = AddOnceTimer(isCustomId, timerId, duration, time, onComplete, (timerReadModel, _) =>
			{
				var value = startValue + diffValue * timerReadModel.Normalized;
				if (timerReadModel.IsPlayed)
					value = targetValue;
				onTickValue(timerReadModel, value);
			}, args);


			// var diffValueSpeed = (targetValue - startValue) / duration;
			// var value = startValue;
			// var id = AddOnceTimer(isCustomId, timerId, duration, time, onComplete, (timerReadModel, deltaTime) =>
			// {
			// 	var diffValueSpeedDeltaTime = diffValueSpeed * deltaTime;
			// 	value += diffValueSpeedDeltaTime;
			// 	if (timerReadModel.IsPlayed)
			// 		value = targetValue;
			// 	onTickValue(timerReadModel, value);
			// }, args);

			return id;
		}


		protected virtual string AddOnceTimer(bool isCustomId, string timerId, float duration, float time, Action<ITimerReadModel> onComplete, Action<ITimerReadModel, float> onTick, params object[] args)
		{
			if (!IsInitialized)
			{
				Debug.LogWarning("[TimerModule::AddOnceTimer] TimerModule is not initialized.");
				return string.Empty;
			}

			return AddTimerToUsingTimerMap(isCustomId, timerId, true, duration, time, onComplete, onTick, args);
		}

		protected virtual string AddLoopTimer(bool isCustomId, string timerId, float duration, float time, Action<ITimerReadModel> onComplete, Action<ITimerReadModel, float> onTick, params object[] args)
		{
			if (!IsInitialized)
			{
				Debug.LogWarning("[TimerModule::AddLoopTimer] TimerModule is not initialized.");
				return string.Empty;
			}

			return AddTimerToUsingTimerMap(isCustomId, timerId, false, duration, time, onComplete, onTick, args);
		}


		protected virtual string AddTimerToUsingTimerMap(bool isCustomId, string customId, bool isOnce, float duration, float time, Action<ITimerReadModel> onComplete, Action<ITimerReadModel, float> onTick, params object[] args)
		{
			var timerId = isCustomId ? customId : Guid.NewGuid().ToString();

			var timer = GetTimerFromUnusingTimer();
			timer.Initialize(timerId, isOnce, duration, time, onComplete, onTick, args);

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
				_unusingTimers.Add(CreateTimer());

			var timer = _unusingTimers.First();
			_unusingTimers.Remove(timer);

			return timer;
		}

		protected virtual Timer CreateTimer() => new Timer();
	}
}