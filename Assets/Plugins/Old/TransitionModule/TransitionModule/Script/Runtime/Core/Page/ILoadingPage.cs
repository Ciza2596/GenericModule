using CizaPageModule;
using CizaUniTask;

namespace CizaTransitionModule
{
    public interface ILoadingPage: IShowingAnimatedImmediately
    {
        UniTask DefaultLoadingAsync();
    }
}