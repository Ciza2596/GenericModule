using System;

namespace SceneModule
{
    public interface ILoadingView
    {
        public void Loading(ILoadSceneAsync loadSceneAsync, Action onComplete);
    }
}
