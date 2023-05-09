using System;
using DG.Tweening;
using GameCore.Infrastructure;

namespace CizaAudioPlayerModule.Implement
{
    public class TimerModuleImp
    {
        private GameCore.Infrastructure.ITimerModule _timerModule;

        public TimerModuleImp(GameCore.Infrastructure.ITimerModule timerModule) => _timerModule = timerModule;

        public string AddOnceTimer(float duration, Action onComplete)
        {
            var id = _timerModule.RegisterOnceTimer(duration, onComplete);
            return id;
        }

        public void RemoveTimer(string id) =>
            _timerModule.UnRegisterOnceTimer(id);
        

        public void AddOnceTimer(float startValue, Action<float> valueSetter, float endValue, float duration,
            Action onComplete = null) =>
            DOTween.To(() => startValue, volume => valueSetter(volume), endValue, duration).OnComplete(() => onComplete?.Invoke());
    }
    
}
