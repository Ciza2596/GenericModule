using Cysharp.Threading.Tasks;
using PageModule;
using UnityEngine;

public class FakePage : MonoBehaviour, IInitializable, IUpdatable, IFixedUpdatable, IReleasable, IBeforeShowable,
    IShowable, IShowActionable, ICompleteShowable, IHidable, IHidingActionable, ICompleteHidable
{
    //public variable
    public bool IsInitializePass { get; private set; }
    public bool IsOnUpdatePass { get; private set; }
    public bool IsOnFixedUpdatePass { get; private set; }
    public bool IsReleasePass { get; private set; }


    public bool IsBeforeShowingPass { get; private set; }
    public bool IsShowPass { get; private set; }
    public bool IsShowingActionPass { get; private set; }
    public bool IsCompleteShowingPass { get; private set; }


    public bool IsHidePass { get; private set; }
    public bool IsHidingActionPass { get; private set; }
    public bool IsCompleteHidingPass { get; private set; }


    //public method
    public void Initialize() => IsInitializePass = true;

    public void OnUpdate(float deltaTime) => IsOnUpdatePass = true;

    public void OnFixedUpdate(float fixedDeltaTime) => IsOnFixedUpdatePass = true;

    public void Release() => IsReleasePass = true;


    public async UniTask BeforeShowing(params object[] parameters)
    {
        IsBeforeShowingPass = true;
        await UniTask.CompletedTask;
    }

    public void Show() => IsShowPass = true;

    public async UniTask ShowingAction()
    {
        IsShowingActionPass = true;
        await UniTask.CompletedTask;
    }

    public void CompleteShowing() => IsCompleteShowingPass = true;


    public void Hide() => IsHidePass = true;

    public async UniTask HidingAction()
    {
        IsHidingActionPass = true;
        await UniTask.CompletedTask;
    }

    public void CompleteHiding() => IsCompleteHidingPass = true;
}