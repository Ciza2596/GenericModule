using Cysharp.Threading.Tasks;

namespace PageModule
{
    public interface IShowingStart
    {
        UniTask OnShowingStart(params object[] parameters);
    }
}