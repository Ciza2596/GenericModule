using Cysharp.Threading.Tasks;
using PageModule;
using UnityEngine;

public class FakePage : MonoBehaviour, IInitializable, ITickable, IFixedTickable, IReleasable, IShowingStart,
    IShowingAnimated, IShowingComplete, IHidingStart, IHidingAnimated, IHidingComplete
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

    public void Tick(float deltaTime) => IsOnUpdatePass = true;

    public void FixedTick(float fixedDeltaTime) => IsOnFixedUpdatePass = true;

    public void Release() => IsReleasePass = true;


    public async UniTask OnShowingStart(params object[] parameters)
    {
        IsBeforeShowingPass = true;
        await UniTask.CompletedTask;
    }

    public async UniTask PlayShowingAnimation()
    {
        IsShowingActionPass = true;
        await UniTask.CompletedTask;
    }

    public void OnShowingComplete() => IsCompleteShowingPass = true;

    public void OnHidingStart() => IsHidePass = true;

    public async UniTask PlayHidingAnimation()
    {
        IsHidingActionPass = true;
        await UniTask.CompletedTask;
    }

    public void OnHidingComplete() => IsCompleteHidingPass = true;
}