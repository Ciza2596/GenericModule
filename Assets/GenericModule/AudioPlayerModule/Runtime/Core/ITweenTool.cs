
using System;

namespace AudioPlayerModule
{
    public interface ITweenTool
    {
        public void Initialize();

        public string StartTimer(float duration);

        public void SetTimerCallBack(string id, Action callback);

        public void StopTimer(string id);
        
        public void To(float startValue, Action<float> valueSetter, float endValue, float duration, Action onComplete = null);
    }
}
