using System;

namespace SceneModule
{
    public interface ILoadingView
    {
        public void Loading(Action loadScene, Action onComplete);
    }
}
