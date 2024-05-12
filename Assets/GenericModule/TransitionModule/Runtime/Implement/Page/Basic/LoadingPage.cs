using Cysharp.Threading.Tasks;

namespace CizaTransitionModule.Implement
{
    public abstract class LoadingPage : TransitionPage, ILoadingPage
    {
        public abstract void PlayShowingAnimationImmediately();

        public abstract UniTask DefaultLoadingAsync();
    }
}