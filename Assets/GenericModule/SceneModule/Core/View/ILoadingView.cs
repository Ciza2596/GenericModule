using System;

namespace SceneModule
{
    public interface ILoadingView
    {
        public void Loading(TransitionController transitionController, Action onComplete);
    }
}
