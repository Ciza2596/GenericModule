using CizaUniTask;

namespace CizaTransitionModule.Implement
{
    public abstract class Presenter : IPresenter
    {
        public virtual UniTask InitializeAsync() =>
            UniTask.CompletedTask;

        public virtual void Complete() { }

        public virtual UniTask ReleaseAsync() =>
            UniTask.CompletedTask;
    }
}