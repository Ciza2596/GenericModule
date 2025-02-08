using Cysharp.Threading.Tasks;

namespace CizaTransitionModule
{
    public interface IPresenter
    {
        UniTask InitializeAsync();

        void Complete();

        UniTask ReleaseAsync();
    }
}