using Cysharp.Threading.Tasks;

namespace CizaTransitionModule.Implement
{
    public abstract class Presenter : IPresenter
    {
        public virtual UniTask InitializeAsync() =>
            UniTask.CompletedTask;

        public virtual void Complete() { }

        public virtual void Release() { }
    }
}