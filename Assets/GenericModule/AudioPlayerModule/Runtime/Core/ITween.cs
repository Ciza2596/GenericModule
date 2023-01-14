
using System;

namespace AudioPlayerModule
{
    public interface ITween
    {
        public string PlayTimer(float duration, Action onComplete);
        
        public void StopTimer(string id);
        
        public void To(float startValue, Action<float> valueSetter, float endValue, float duration, Action onComplete = null);
    }
}
