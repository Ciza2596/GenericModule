using Cysharp.Threading.Tasks;

namespace CizaTransitionModule.Implement
{
    public abstract class TransitionInPage : TransitionPage, ITransitionInPage
    {
        public abstract UniTask PlayShowingAnimationAsync();
    }
}