
using System;

namespace CizaAudioPlayerModule
{
    public interface ITimerModule
    {
        public string AddOnceTimer(float duration, Action onComplete);
        
        public void StopTimer(string id);
        
        public void AddOnceTimer(float startValue, Action<float> valueSetter, float endValue, float duration, Action onComplete = null);
    }
}
