using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CizaTimerModule
{
    public class TimerModule
    {
        private Dictionary<string, Timer> _timerMap;

        public bool IsInitialized => _timerMap is not null;

        public void Initialize()
        {
            if (IsInitialized)
            {
                Debug.LogWarning("[TimerModule::Initialize] TimerModule is initialized.");
                return;
            }

            _timerMap = new();
        }


        public void Release()
        {
            if (!IsInitialized)
            {
                Debug.LogWarning("[TimerModule::Release] TimerModule is not initialized.");
                return;
            }

            _timerMap.Clear();
            _timerMap = null;
        }

        public void Tick(float deltaTime)
        {
            if (!IsInitialized)
            {
                Debug.LogWarning("[TimerModule::Tick] TimerModule is not initialized.");
                return;
            }

            foreach (var timer in _timerMap.Values.ToArray())
            {
                timer.AddDeltaTime(deltaTime);

                if (timer.Time >= timer.TriggerTime)
                {
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

            if (!_timerMap.ContainsKey(timerId))
                return false;

            timerReadModel = _timerMap[timerId];

            return true;
        }

        public string AddOnceTimer(float triggerTime, Action<string> action)
        {
            if (!IsInitialized)
            {
                Debug.LogWarning("[TimerModule::AddOnceTimer] TimerModule is not initialized.");
                return string.Empty;
            }

            return AddTimer(true, triggerTime, action);
        }


        public string AddLoopTimer(float triggerTime, Action<string> action)
        {
            if (!IsInitialized)
            {
                Debug.LogWarning("[TimerModule::AddLoopTimer] TimerModule is not initialized.");
                return string.Empty;
            }

            return AddTimer(false, triggerTime, action);
        }


        public void RemoveTimer(string timerId)
        {
            if (!IsInitialized)
            {
                Debug.LogWarning("[TimerModule::RemoveTimer] TimerModule is not initialized.");
                return;
            }

            if (!_timerMap.ContainsKey(timerId))
            {
                Debug.LogWarning($"[TimerModule::RemoveTimer] Timer is not found by timerId: {timerId}.");
                return;
            }

            _timerMap.Remove(timerId);
        }


        // private method
        private string AddTimer(bool isOnce, float triggerTime, Action<string> action)
        {
            var timerId = Guid.NewGuid().ToString();
            var timerData = new Timer(timerId, isOnce, triggerTime, action);

            _timerMap.Add(timerId, timerData);

            return timerId;
        }
    }
}