using System;
using DG.Tweening;
using GameCore.Infrastructure;

namespace AudioPlayerModule.Implement
{
    public class TweenImp : ITween
    {
        private ITimerModule _timerModule;

        public TweenImp(ITimerModule timerModule) => _timerModule = timerModule;

        public string StartTimer(float duration, Action callback)
        {
            var id = _timerModule.RegisterOnceTimer(duration, callback);
            return id;
        }

        public void StopTimer(string id) =>
            _timerModule.UnRegisterOnceTimer(id);
        

        public void To(float startValue, Action<float> valueSetter, float endValue, float duration,
            Action onComplete = null) =>
            DOTween.To(() => startValue, volume => valueSetter(volume), endValue, duration).OnComplete(() => onComplete());
    }
    
}
