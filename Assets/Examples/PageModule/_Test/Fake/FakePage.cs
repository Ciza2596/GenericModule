using Cysharp.Threading.Tasks;
using CizaPageModule;
using UnityEngine;

public class FakePage : MonoBehaviour, IInitializable, ITickable, IFixedTickable, IReleasable, IShowingPrepare, IShowingStart, IShowingAnimated, IShowingAnimatedImmediately, IShowingComplete, IHidingStart, IHidingAnimated, IHidingAnimatedImmediately, IHidingComplete
{
	//public variable
	public       bool   IsPassInitialize { get; private set; }
	public       bool   IsPassTick       { get; private set; }
	public       bool   IsPassFixedTick  { get; private set; }
	public const string IS_PASS_RELEASE_CREATE_GAME_OBJECT_NAME = "FakeIsReleased";

	public bool IsPassOnShowingPrepare { get; private set; }
	public bool IsPassOnShowingStart   { get; private set; }

	public bool IsPassPlayShowingAnimation            { get; private set; }
	public bool IsPassPlayShowingAnimationImmediately { get; private set; }

	public bool IsPassOnShowingComplete { get; private set; }

	public bool IsPassOnHidingStart                  { get; private set; }
	
	public bool IsPassPlayHidingAnimation            { get; private set; }
	public bool IsPassPlayHidingAnimationImmediately { get; private set; }
	
	public bool IsPassOnHidingComplete               { get; private set; }

	//public method
	public UniTask InitializeAsync(params object[] parameters)
	{
		IsPassInitialize = true;
		return UniTask.CompletedTask;
	}

	public void Tick(float deltaTime) => IsPassTick = true;

	public void FixedTick(float fixedDeltaTime) => IsPassFixedTick = true;

	public void Release()
	{
		new GameObject(IS_PASS_RELEASE_CREATE_GAME_OBJECT_NAME);
	}

	public UniTask ShowingPrepareAsync(params object[] parameters)
	{
		IsPassOnShowingPrepare = true;
		return UniTask.CompletedTask;
	}
	
	public void ShowingStart() =>
		IsPassOnShowingStart = true;

	public UniTask PlayShowingAnimationAsync()
	{
		IsPassPlayShowingAnimation = true;
		return UniTask.CompletedTask;
	}

	public void PlayShowingAnimationImmediately() =>
		IsPassPlayShowingAnimationImmediately = true;

	public void ShowingComplete() =>
		IsPassOnShowingComplete = true;

	public void HidingStart() =>
		IsPassOnHidingStart = true;

	public UniTask PlayHidingAnimationAsync()
	{
		IsPassPlayHidingAnimation = true;
		return UniTask.CompletedTask;
	}

	public void PlayHidingAnimationImmediately() =>
		IsPassPlayHidingAnimationImmediately = true;

	public void HidingComplete() =>
		IsPassOnHidingComplete = true;
}
