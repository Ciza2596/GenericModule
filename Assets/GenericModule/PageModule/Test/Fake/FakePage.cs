using Cysharp.Threading.Tasks;
using PageModule;
using UnityEngine;

public class FakePage : MonoBehaviour, IInitializable, IUpdatable, IFixedUpdatable, IReleasable, IBeforeShowable,
    IShowable,
    IShowActionable, ICompleteShowable, IHidable, IHidingActionable, ICompleteHidable
{
    public void Initialize(params object[] parameters)
    {
    }

    public void OnUpdate(float deltaTime)
    {
    }

    public void OnFixedUpdate(float fixedDeltaTime)
    {
    }

    public void Release()
    {
    }

    public async UniTask BeforeShowing(params object[] parameters)
    {
        await UniTask.CompletedTask;
    }

    public void Show()
    {
    }

    public async UniTask ShowingAction()
    {
        await UniTask.CompletedTask;
    }

    public void CompleteShowing()
    {
    }

    public void Hide()
    {
    }

    public async UniTask HidingAction()
    {
        await UniTask.CompletedTask;
    }

    public void CompleteHiding()
    {
    }
}