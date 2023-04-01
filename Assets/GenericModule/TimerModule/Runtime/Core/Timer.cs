using System;

namespace CizaTimerModule
{
    internal class Timer : TimerReadModel
    {
        private Action<string> _action;

        public Timer(string id, bool isOnce, float triggerTime, Action<string> action)
        {
            Id = id;
            IsOnce = isOnce;
            TriggerTime = triggerTime;
            _action = action;
        }

        public string Id { get; }

        
        public bool IsOnce { get; }
        public float TriggerTime { get; }
        public float Time { get; private set; }
        
        
        public void AddDeltaTime(float deltaTime) =>
            Time += deltaTime;

        public void ResetTime() => Time = 0;

        public void Invoke() => _action?.Invoke(Id);
    }
}