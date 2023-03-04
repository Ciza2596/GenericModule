using Cysharp.Threading.Tasks;

namespace PageModule
{
    public interface IInitializable
    {
        UniTask Initialize(params object[] parameters);
    }
}