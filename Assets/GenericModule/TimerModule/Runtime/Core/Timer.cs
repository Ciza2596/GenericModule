using System;

namespace CizaTimerModule
{
    internal class Timer : TimerReadModel
    {
        private Action<string> _action;

        public void Initialize(string id, bool isOnce, float triggerTime, Action<string> action)
        {
            Id = id;
            IsOnce = isOnce;
            TriggerTime = triggerTime;
            _action = action;
        }

        public string Id { get; private set; }

        
        public bool IsOnce { get; private set; }
        public float TriggerTime { get; private set; }
        public float Time { get; private set; }
        
        
        public void AddDeltaTime(float deltaTime) =>
            Time += deltaTime;

        public void ResetTime() => Time = 0;

        public void Invoke() => _action?.Invoke(Id);
    }
}