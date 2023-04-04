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
                timer.AddDeltaTime(deltaTime);

                if (timer.Time >= timer.TriggerTime)
                {
                    timer.ResetTime();
                    timer.Invoke();
                    if (timer.IsOnce)
                        RemoveTimer(timer.Id);
                }
            }
        }


        public bool TryGetTimerReadModel(string timerId, out TimerReadModel timerReadModel)
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

        public string AddOnceTimer(float triggerTime, Action<string> action)
        {
            if (!IsInitialized)
            {
                Debug.LogWarning("[TimerModule::AddOnceTimer] TimerModule is not initialized.");
                return string.Empty;
            }

            return AddTimerToUsingTimerMap(true, triggerTime, action);
        }


        public string AddLoopTimer(float triggerTime, Action<string> action)
        {
            if (!IsInitialized)
            {
                Debug.LogWarning("[TimerModule::AddLoopTimer] TimerModule is not initialized.");
                return string.Empty;
            }

            return AddTimerToUsingTimerMap(false, triggerTime, action);
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
        }


        // private method
        private string AddTimerToUsingTimerMap(bool isOnce, float triggerTime, Action<string> action)
        {
            var timerId = Guid.NewGuid().ToString();
            
            var timer = GetTimerFromUnusingTimer();
            timer.Initialize(timerId, isOnce, triggerTime, action);
            
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