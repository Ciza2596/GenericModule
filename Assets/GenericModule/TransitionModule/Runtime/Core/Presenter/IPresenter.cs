using Cysharp.Threading.Tasks;

namespace CizaTransitionModule
{
    public interface IPresenter
    {
        UniTask InitializeAsync();

        void Complete();

        void Release();
    }
}