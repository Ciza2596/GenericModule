using CizaPageModule;
using Cysharp.Threading.Tasks;

namespace CizaTransitionModule
{
    public interface ILoadingPage: IShowingAnimatedImmediately
    {
        UniTask DefaultLoadingAsync();
    }
}