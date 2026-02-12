using CizaUniTask;

namespace CizaTransitionModule.Implement
{
    public abstract class TransitionOutPage : TransitionPage, ITransitionOutPage
    {
        public abstract UniTask PlayShowingAnimationAsync();
    }
}