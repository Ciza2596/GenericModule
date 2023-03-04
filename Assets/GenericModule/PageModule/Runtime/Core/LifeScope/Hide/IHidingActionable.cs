using Cysharp.Threading.Tasks;

namespace PageModule
{
    public interface IHidingActionable
    {
        UniTask HidingAction();
    }
}