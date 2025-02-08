using System;

namespace CizaSceneModule
{
    public interface ITransitionView
    {
        public void Play(Action onComplete);
    }
}
