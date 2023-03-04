using Cysharp.Threading.Tasks;

namespace PageModule
{
    public interface IBeforeShowable
    {
        UniTask BeforeShowing(params object[] parameters);
    }
}