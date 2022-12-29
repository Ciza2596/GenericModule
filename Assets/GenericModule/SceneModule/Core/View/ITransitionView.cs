using System;

namespace SceneModule
{
    public interface ITransitionView
    {
        public void Play(Action onComplete);
    }
}
