using Cysharp.Threading.Tasks;
using CizaPageModule;
using UnityEngine;

public class FakePage : MonoBehaviour, IInitializable, ITickable, IFixedTickable, IReleasable, IShowingStart, IShowingAnimated, IShowingComplete, IHidingStart, IHidingAnimated, IHidingComplete
{
	//public variable
	public       bool   IsPassInitialize { get; private set; }
	public       bool   IsPassTick       { get; private set; }
	public       bool   IsPassFixedTick  { get; private set; }
	public const string IS_PASS_RELEASE_CREATE_GAME_OBJECT_NAME = "FakeIsReleased";

	public bool IsPassOnShowingStart       { get; private set; }
	public bool IsPassPlayShowingAnimation { get; private set; }
	public bool IsPassOnShowingComplete    { get; private set; }

	public bool IsPassOnHidingStart       { get; private set; }
	public bool IsPassPlayHidingAnimation { get; private set; }
	public bool IsPassOnHidingComplete    { get; private set; }

	//public method
	public UniTask Initialize(params object[] parameters)
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

	public async UniTask OnShowingStart()
	{
		IsPassOnShowingStart = true;
		await UniTask.CompletedTask;
	}

	public async UniTask PlayShowingAnimation()
	{
		IsPassPlayShowingAnimation = true;
		await UniTask.CompletedTask;
	}

	public void OnShowingComplete() => IsPassOnShowingComplete = true;

	public void OnHidingStart() => IsPassOnHidingStart = true;

	public async UniTask PlayHidingAnimation()
	{
		IsPassPlayHidingAnimation = true;
		await UniTask.CompletedTask;
	}

	public void OnHidingComplete() => IsPassOnHidingComplete = true;
}
