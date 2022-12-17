using System;
using AudioPlayerModule;
using DG.Tweening;


public class TweenToolImp : ITweenTool
{
    public void Initialize()
    {
    }

    public string StartTimer(float duration)
    {
        return "";
    }

    public void SetTimerCallBack(string id, Action callback)
    {
    }

    public void StopTimer(string id)
    {
        
    }

    public void To(float startValue, Action<float> valueSetter, float endValue, float duration,
        Action onComplete = null) =>
        DOTween.To(() => startValue, volume => valueSetter(volume), endValue, duration).OnComplete(() => onComplete());
}