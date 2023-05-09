
using System;

namespace CizaAudioPlayerModule
{
    public interface ITimerModule
    {
        public string AddOnceTimer(float duration, Action onComplete);
        
        public void RemoveTimer(string id);
        
        public void AddOnceTimer(float startValue, float endValue, float duration, Action<float> valueSetter, Action onComplete = null);
    }
}
