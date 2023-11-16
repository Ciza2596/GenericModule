using System;
using System.Collections.Generic;
using NUnit.Framework;
using CizaPageModule;
using UnityEngine;
using Object = UnityEngine.Object;

public class PageContainerTest
{
	private static readonly string _pageKey = nameof(FakePage);

	private PageContainer _pageContainer;
	private Transform     _pageGameObjectRootTransform;

	[SetUp]
	public void SetUp()
	{
		var pageGameObjectRoot = new GameObject();
		_pageGameObjectRootTransform = pageGameObjectRoot.transform;

		_pageContainer = new PageContainer();

		_pageContainer.Initialize(_pageGameObjectRootTransform, CreatePagePrefabs(new[] { typeof(FakePage) }));
	}

	[TearDown]
	public void TearDown()
	{
		_pageContainer.DestroyAll();
		_pageContainer = null;

		var pageGameObjectRootTransform = _pageGameObjectRootTransform;
		_pageGameObjectRootTransform = null;
		Object.DestroyImmediate(pageGameObjectRootTransform.gameObject);

		var gameObject = GameObject.Find(FakePage.IS_PASS_RELEASE_CREATE_GAME_OBJECT_NAME);
		if (gameObject != null)
			Object.DestroyImmediate(gameObject);
	}

	[Test]
	public async void _01_Create()
	{
		//act
		await _pageContainer.CreateAsync<FakePage>(_pageKey);

		//assert
		Check_Page_Is_Created<FakePage>(_pageKey);
		Check_IsPassInitialize<FakePage>(_pageKey, true);
	}

	[Test]
	public void _02_Destroy()
	{
		//arrange
		Create_And_Check_Page_Is_Created<FakePage>(_pageKey);
		Check_IsPassRelease(_pageKey, false);

		//act
		_pageContainer.Destroy(_pageKey);

		//assert
		Check_Page_Is_Destroyed<FakePage>(_pageKey);
		Check_IsPassRelease(_pageKey, true);
	}

	[Test]
	public void _03_DestroyAll()
	{
		//arrange
		Create_And_Check_Page_Is_Created<FakePage>(_pageKey);
		Check_IsPassRelease(_pageKey, false);

		//act
		_pageContainer.DestroyAll();

		//assert
		Check_Page_Is_Destroyed<FakePage>(_pageKey);
		Check_IsPassRelease(_pageKey, true);
	}

	[Test]
	public async void _04_Show_A_Page()
	{
		//arrange
		Create_And_Check_Page_Is_Created<FakePage>(_pageKey);
		Check_Page_Is_Invisible(_pageKey);

		Check_IsPassOnShowingStart<FakePage>(_pageKey, false);
		Check_IsPassPlayShowingAnimation<FakePage>(_pageKey, false);
		Check_IsPassPlayShowingAnimationImmediately<FakePage>(_pageKey, false);
		Check_IsPassOnShowingComplete<FakePage>(_pageKey, false);

		//act
		await _pageContainer.ShowAsync(_pageKey);

		//assert
		Check_Page_Is_Visible(_pageKey);

		Check_IsPassOnShowingStart<FakePage>(_pageKey, true);
		Check_IsPassPlayShowingAnimation<FakePage>(_pageKey, true);
		Check_IsPassPlayShowingAnimationImmediately<FakePage>(_pageKey, false);
		Check_IsPassOnShowingComplete<FakePage>(_pageKey, true);
	}

	[Test]
	public async void _05_Show_A_Page_With_Three_Step()
	{
		//arrange
		Create_And_Check_Page_Is_Created<FakePage>(_pageKey);
		Check_Page_Is_Invisible(_pageKey);

		Check_IsPassOnShowingStart<FakePage>(_pageKey, false);
		Check_IsPassPlayShowingAnimation<FakePage>(_pageKey, false);
		Check_IsPassPlayShowingAnimationImmediately<FakePage>(_pageKey, false);
		Check_IsPassOnShowingComplete<FakePage>(_pageKey, false);

		//act
		await _pageContainer.OnlyCallShowingStartAsync(_pageKey);
		await _pageContainer.ShowAsync(_pageKey, isIncludeShowingComplete: false);
		_pageContainer.OnlyCallShowingComplete(_pageKey);

		//assert
		Check_Page_Is_Visible(_pageKey);

		Check_IsPassOnShowingStart<FakePage>(_pageKey, true);
		Check_IsPassPlayShowingAnimation<FakePage>(_pageKey, true);
		Check_IsPassPlayShowingAnimationImmediately<FakePage>(_pageKey, false);
		Check_IsPassOnShowingComplete<FakePage>(_pageKey, true);
	}

	[Test]
	public async void _06_ShowImmediately_A_Page()
	{
		//arrange
		Create_And_Check_Page_Is_Created<FakePage>(_pageKey);
		Check_Page_Is_Invisible(_pageKey);

		Check_IsPassOnShowingStart<FakePage>(_pageKey, false);
		Check_IsPassPlayShowingAnimation<FakePage>(_pageKey, false);
		Check_IsPassPlayShowingAnimationImmediately<FakePage>(_pageKey, false);
		Check_IsPassOnShowingComplete<FakePage>(_pageKey, false);

		//act
		await _pageContainer.ShowImmediatelyAsync(_pageKey);

		//assert
		Check_Page_Is_Visible(_pageKey);

		Check_IsPassOnShowingStart<FakePage>(_pageKey, true);
		Check_IsPassPlayShowingAnimation<FakePage>(_pageKey, false);
		Check_IsPassPlayShowingAnimationImmediately<FakePage>(_pageKey, true);
		Check_IsPassOnShowingComplete<FakePage>(_pageKey, true);
	}

	[Test]
	public async void _07_ShowImmediately_A_Page_With_Three_Step()
	{
		//arrange
		Create_And_Check_Page_Is_Created<FakePage>(_pageKey);
		Check_Page_Is_Invisible(_pageKey);

		Check_IsPassOnShowingStart<FakePage>(_pageKey, false);
		Check_IsPassPlayShowingAnimation<FakePage>(_pageKey, false);
		Check_IsPassPlayShowingAnimationImmediately<FakePage>(_pageKey, false);
		Check_IsPassOnShowingComplete<FakePage>(_pageKey, false);

		//act
		await _pageContainer.OnlyCallShowingStartAsync(_pageKey);
		await _pageContainer.ShowImmediatelyAsync(_pageKey, isIncludeShowingComplete: false);
		_pageContainer.OnlyCallShowingComplete(_pageKey);

		//assert
		Check_Page_Is_Visible(_pageKey);

		Check_IsPassOnShowingStart<FakePage>(_pageKey, true);
		Check_IsPassPlayShowingAnimation<FakePage>(_pageKey, false);
		Check_IsPassPlayShowingAnimationImmediately<FakePage>(_pageKey, true);
		Check_IsPassOnShowingComplete<FakePage>(_pageKey, true);
	}

	[Test]
	public async void _08_Show_Some_Pages()
	{
		//arrange
		Create_And_Check_Page_Is_Created<FakePage>(_pageKey);
		Check_Page_Is_Invisible(_pageKey);

		Check_IsPassOnShowingStart<FakePage>(_pageKey, false);
		Check_IsPassPlayShowingAnimation<FakePage>(_pageKey, false);
		Check_IsPassPlayShowingAnimationImmediately<FakePage>(_pageKey, false);
		Check_IsPassOnShowingComplete<FakePage>(_pageKey, false);

		//act
		await _pageContainer.ShowAsync(new[] { _pageKey });

		//assert
		Check_Page_Is_Visible(_pageKey);

		Check_IsPassOnShowingStart<FakePage>(_pageKey, true);
		Check_IsPassPlayShowingAnimation<FakePage>(_pageKey, true);
		Check_IsPassPlayShowingAnimationImmediately<FakePage>(_pageKey, false);
		Check_IsPassOnShowingComplete<FakePage>(_pageKey, true);
	}

	[Test]
	public async void _09_Show_Some_Pages_With_Three_Step()
	{
		//arrange
		Create_And_Check_Page_Is_Created<FakePage>(_pageKey);
		Check_Page_Is_Invisible(_pageKey);

		Check_IsPassOnShowingStart<FakePage>(_pageKey, false);
		Check_IsPassPlayShowingAnimation<FakePage>(_pageKey, false);
		Check_IsPassPlayShowingAnimationImmediately<FakePage>(_pageKey, false);
		Check_IsPassOnShowingComplete<FakePage>(_pageKey, false);

		//act
		await _pageContainer.OnlyCallShowingStartAsync(_pageKey);
		await _pageContainer.ShowAsync(new[] { _pageKey }, isIncludeHidingComplete: false);
		_pageContainer.OnlyCallShowingComplete(new[] { _pageKey });

		//assert
		Check_Page_Is_Visible(_pageKey);

		Check_IsPassOnShowingStart<FakePage>(_pageKey, true);
		Check_IsPassPlayShowingAnimation<FakePage>(_pageKey, true);
		Check_IsPassPlayShowingAnimationImmediately<FakePage>(_pageKey, false);
		Check_IsPassOnShowingComplete<FakePage>(_pageKey, true);
	}

	[Test]
	public async void _10_ShowImmediately_Some_Pages()
	{
		//arrange
		Create_And_Check_Page_Is_Created<FakePage>(_pageKey);
		Check_Page_Is_Invisible(_pageKey);

		Check_IsPassOnShowingStart<FakePage>(_pageKey, false);
		Check_IsPassPlayShowingAnimation<FakePage>(_pageKey, false);
		Check_IsPassPlayShowingAnimationImmediately<FakePage>(_pageKey, false);
		Check_IsPassOnShowingComplete<FakePage>(_pageKey, false);

		//act
		await _pageContainer.OnlyCallShowingStartAsync(_pageKey);
		await _pageContainer.ShowImmediatelyAsync(new[] { _pageKey }, isIncludeHidingComplete: false);
		_pageContainer.OnlyCallShowingComplete(new[] { _pageKey });

		//assert
		Check_Page_Is_Visible(_pageKey);

		Check_IsPassOnShowingStart<FakePage>(_pageKey, true);
		Check_IsPassPlayShowingAnimation<FakePage>(_pageKey, false);
		Check_IsPassPlayShowingAnimationImmediately<FakePage>(_pageKey, true);
		Check_IsPassOnShowingComplete<FakePage>(_pageKey, true);
	}

	[Test]
	public async void _11_ShowImmediately_Some_Pages_With_Three_Step()
	{
		//arrange
		Create_And_Check_Page_Is_Created<FakePage>(_pageKey);
		Check_Page_Is_Invisible(_pageKey);

		Check_IsPassOnShowingStart<FakePage>(_pageKey, false);
		Check_IsPassPlayShowingAnimation<FakePage>(_pageKey, false);
		Check_IsPassPlayShowingAnimationImmediately<FakePage>(_pageKey, false);
		Check_IsPassOnShowingComplete<FakePage>(_pageKey, false);

		//act
		await _pageContainer.ShowImmediatelyAsync(new[] { _pageKey });

		//assert
		Check_Page_Is_Visible(_pageKey);

		Check_IsPassOnShowingStart<FakePage>(_pageKey, true);
		Check_IsPassPlayShowingAnimation<FakePage>(_pageKey, false);
		Check_IsPassPlayShowingAnimationImmediately<FakePage>(_pageKey, true);
		Check_IsPassOnShowingComplete<FakePage>(_pageKey, true);
	}

	[Test]
	public async void _12_Hide_A_Page()
	{
		//arrange
		Create_And_Check_Page_Is_Created<FakePage>(_pageKey);
		Check_Page_Is_Invisible(_pageKey);

		Show_Immediately_And_Check_Page_Is_Visible(_pageKey);

		Check_IsPassOnHidingStart<FakePage>(_pageKey, false);
		Check_IsPassPlayHidingAnimation<FakePage>(_pageKey, false);
		Check_IsPassPlayHidingAnimationImmediately<FakePage>(_pageKey, false);
		Check_IsPassOnHidingComplete<FakePage>(_pageKey, false);

		//act
		await _pageContainer.HideAsync(_pageKey);

		//assert
		Check_Page_Is_Invisible(_pageKey);

		Check_IsPassOnHidingStart<FakePage>(_pageKey, true);
		Check_IsPassPlayHidingAnimation<FakePage>(_pageKey, true);
		Check_IsPassPlayHidingAnimationImmediately<FakePage>(_pageKey, false);
		Check_IsPassOnHidingComplete<FakePage>(_pageKey, true);
	}

	[Test]
	public async void _13_Hide_A_Page_With_Three_Step()
	{
		//arrange
		Create_And_Check_Page_Is_Created<FakePage>(_pageKey);
		Check_Page_Is_Invisible(_pageKey);

		Show_Immediately_And_Check_Page_Is_Visible(_pageKey);

		Check_IsPassOnHidingStart<FakePage>(_pageKey, false);
		Check_IsPassPlayHidingAnimation<FakePage>(_pageKey, false);
		Check_IsPassPlayHidingAnimationImmediately<FakePage>(_pageKey, false);
		Check_IsPassOnHidingComplete<FakePage>(_pageKey, false);

		//act
		_pageContainer.OnlyCallHidingStart(_pageKey);
		await _pageContainer.HideAsync(_pageKey, isIncludeHidingComplete: false);
		_pageContainer.OnlyCallHidingComplete(_pageKey);

		//assert
		Check_Page_Is_Invisible(_pageKey);

		Check_IsPassOnHidingStart<FakePage>(_pageKey, true);
		Check_IsPassPlayHidingAnimation<FakePage>(_pageKey, true);
		Check_IsPassPlayHidingAnimationImmediately<FakePage>(_pageKey, false);
		Check_IsPassOnHidingComplete<FakePage>(_pageKey, true);
	}

	[Test]
	public void _14_HideImmediately_A_Page()
	{
		//arrange
		Create_And_Check_Page_Is_Created<FakePage>(_pageKey);
		Check_Page_Is_Invisible(_pageKey);

		Show_Immediately_And_Check_Page_Is_Visible(_pageKey);

		Check_IsPassOnHidingStart<FakePage>(_pageKey, false);
		Check_IsPassPlayHidingAnimation<FakePage>(_pageKey, false);
		Check_IsPassPlayHidingAnimationImmediately<FakePage>(_pageKey, false);
		Check_IsPassOnHidingComplete<FakePage>(_pageKey, false);

		//act
		_pageContainer.HideImmediately(_pageKey);

		//assert
		Check_Page_Is_Invisible(_pageKey);

		Check_IsPassOnHidingStart<FakePage>(_pageKey, true);
		Check_IsPassPlayHidingAnimation<FakePage>(_pageKey, false);
		Check_IsPassPlayHidingAnimationImmediately<FakePage>(_pageKey, true);
		Check_IsPassOnHidingComplete<FakePage>(_pageKey, true);
	}

	[Test]
	public void _15_HideImmediately_A_Page_With_Three_Step()
	{
		//arrange
		Create_And_Check_Page_Is_Created<FakePage>(_pageKey);
		Check_Page_Is_Invisible(_pageKey);

		Show_Immediately_And_Check_Page_Is_Visible(_pageKey);

		Check_IsPassOnHidingStart<FakePage>(_pageKey, false);
		Check_IsPassPlayHidingAnimation<FakePage>(_pageKey, false);
		Check_IsPassPlayHidingAnimationImmediately<FakePage>(_pageKey, false);
		Check_IsPassOnHidingComplete<FakePage>(_pageKey, false);

		//act
		_pageContainer.OnlyCallHidingStart(_pageKey);
		_pageContainer.HideImmediately(_pageKey, isIncludeHidingComplete: false);
		_pageContainer.OnlyCallHidingComplete(_pageKey);

		//assert
		Check_Page_Is_Invisible(_pageKey);

		Check_IsPassOnHidingStart<FakePage>(_pageKey, true);
		Check_IsPassPlayHidingAnimation<FakePage>(_pageKey, false);
		Check_IsPassPlayHidingAnimationImmediately<FakePage>(_pageKey, true);
		Check_IsPassOnHidingComplete<FakePage>(_pageKey, true);
	}

	[Test]
	public async void _16_Hide_Some_Pages()
	{
		//arrange
		Create_And_Check_Page_Is_Created<FakePage>(_pageKey);
		Check_Page_Is_Invisible(_pageKey);

		Show_Immediately_And_Check_Page_Is_Visible(_pageKey);

		Check_IsPassOnHidingStart<FakePage>(_pageKey, false);
		Check_IsPassPlayHidingAnimation<FakePage>(_pageKey, false);
		Check_IsPassPlayHidingAnimationImmediately<FakePage>(_pageKey, false);
		Check_IsPassOnHidingComplete<FakePage>(_pageKey, false);

		//act
		await _pageContainer.HideAsync(new[] { _pageKey });

		//assert
		Check_Page_Is_Invisible(_pageKey);

		Check_IsPassOnHidingStart<FakePage>(_pageKey, true);
		Check_IsPassPlayHidingAnimation<FakePage>(_pageKey, true);
		Check_IsPassPlayHidingAnimationImmediately<FakePage>(_pageKey, false);
		Check_IsPassOnHidingComplete<FakePage>(_pageKey, true);
	}

	[Test]
	public async void _17_Hide_Some_Pages_With_Three_Step()
	{
		//arrange
		Create_And_Check_Page_Is_Created<FakePage>(_pageKey);
		Check_Page_Is_Invisible(_pageKey);

		Show_Immediately_And_Check_Page_Is_Visible(_pageKey);

		Check_IsPassOnHidingStart<FakePage>(_pageKey, false);
		Check_IsPassPlayHidingAnimation<FakePage>(_pageKey, false);
		Check_IsPassPlayHidingAnimationImmediately<FakePage>(_pageKey, false);
		Check_IsPassOnHidingComplete<FakePage>(_pageKey, false);

		//act
		_pageContainer.OnlyCallHidingStart(_pageKey);
		await _pageContainer.HideAsync(new[] { _pageKey }, isIncludeHidingComplete: false);
		_pageContainer.OnlyCallHidingComplete(new[] { _pageKey });

		//assert
		Check_Page_Is_Invisible(_pageKey);

		Check_IsPassOnHidingStart<FakePage>(_pageKey, true);
		Check_IsPassPlayHidingAnimation<FakePage>(_pageKey, true);
		Check_IsPassPlayHidingAnimationImmediately<FakePage>(_pageKey, false);
		Check_IsPassOnHidingComplete<FakePage>(_pageKey, true);
	}

	[Test]
	public void _18_HideImmediately_Some_Pages()
	{
		//arrange
		Create_And_Check_Page_Is_Created<FakePage>(_pageKey);
		Check_Page_Is_Invisible(_pageKey);

		Show_Immediately_And_Check_Page_Is_Visible(_pageKey);

		Check_IsPassOnHidingStart<FakePage>(_pageKey, false);
		Check_IsPassPlayHidingAnimation<FakePage>(_pageKey, false);
		Check_IsPassPlayHidingAnimationImmediately<FakePage>(_pageKey, false);
		Check_IsPassOnHidingComplete<FakePage>(_pageKey, false);

		//act
		_pageContainer.HideImmediately(new[] { _pageKey });

		//assert
		Check_Page_Is_Invisible(_pageKey);

		Check_IsPassOnHidingStart<FakePage>(_pageKey, true);
		Check_IsPassPlayHidingAnimation<FakePage>(_pageKey, false);
		Check_IsPassPlayHidingAnimationImmediately<FakePage>(_pageKey, true);
		Check_IsPassOnHidingComplete<FakePage>(_pageKey, true);
	}

	[Test]
	public void _19_HideImmediately_Some_Pages_With_Three_Step()
	{
		//arrange
		Create_And_Check_Page_Is_Created<FakePage>(_pageKey);
		Check_Page_Is_Invisible(_pageKey);

		Show_Immediately_And_Check_Page_Is_Visible(_pageKey);

		Check_IsPassOnHidingStart<FakePage>(_pageKey, false);
		Check_IsPassPlayHidingAnimation<FakePage>(_pageKey, false);
		Check_IsPassPlayHidingAnimationImmediately<FakePage>(_pageKey, false);
		Check_IsPassOnHidingComplete<FakePage>(_pageKey, false);

		//act
		_pageContainer.OnlyCallHidingStart(_pageKey);
		_pageContainer.HideImmediately(new[] { _pageKey }, isIncludeHidingComplete: false);
		_pageContainer.OnlyCallHidingComplete(new[] { _pageKey });

		//assert
		Check_Page_Is_Invisible(_pageKey);

		Check_IsPassOnHidingStart<FakePage>(_pageKey, true);
		Check_IsPassPlayHidingAnimation<FakePage>(_pageKey, false);
		Check_IsPassPlayHidingAnimationImmediately<FakePage>(_pageKey, true);
		Check_IsPassOnHidingComplete<FakePage>(_pageKey, true);
	}

	[Test]
	public async void _20_HideAll()
	{
		//arrange
		Create_And_Check_Page_Is_Created<FakePage>(_pageKey);
		Check_Page_Is_Invisible(_pageKey);

		Show_Immediately_And_Check_Page_Is_Visible(_pageKey);

		Check_IsPassOnHidingStart<FakePage>(_pageKey, false);
		Check_IsPassPlayHidingAnimation<FakePage>(_pageKey, false);
		Check_IsPassPlayHidingAnimationImmediately<FakePage>(_pageKey, false);
		Check_IsPassOnHidingComplete<FakePage>(_pageKey, false);

		//act
		await _pageContainer.HideAllAsync();

		//assert
		Check_Page_Is_Invisible(_pageKey);

		Check_IsPassOnHidingStart<FakePage>(_pageKey, true);
		Check_IsPassPlayHidingAnimation<FakePage>(_pageKey, true);
		Check_IsPassPlayHidingAnimationImmediately<FakePage>(_pageKey, false);
		Check_IsPassOnHidingComplete<FakePage>(_pageKey, true);
	}

	[Test]
	public async void _21_HideAll_With_Three_Step()
	{
		//arrange
		Create_And_Check_Page_Is_Created<FakePage>(_pageKey);
		Check_Page_Is_Invisible(_pageKey);

		Show_Immediately_And_Check_Page_Is_Visible(_pageKey);

		Check_IsPassOnHidingStart<FakePage>(_pageKey, false);
		Check_IsPassPlayHidingAnimation<FakePage>(_pageKey, false);
		Check_IsPassPlayHidingAnimationImmediately<FakePage>(_pageKey, false);
		Check_IsPassOnHidingComplete<FakePage>(_pageKey, false);

		//act
		_pageContainer.OnlyCallHidingStart(_pageKey);
		await _pageContainer.HideAllAsync(isIncludeHidingComplete: false);
		_pageContainer.OnlyCallAllHidingComplete();

		//assert
		Check_Page_Is_Invisible(_pageKey);

		Check_IsPassOnHidingStart<FakePage>(_pageKey, true);
		Check_IsPassPlayHidingAnimation<FakePage>(_pageKey, true);
		Check_IsPassPlayHidingAnimationImmediately<FakePage>(_pageKey, false);
		Check_IsPassOnHidingComplete<FakePage>(_pageKey, true);
	}

	[Test]
	public void _22_HideAllImmediately()
	{
		//arrange
		Create_And_Check_Page_Is_Created<FakePage>(_pageKey);
		Check_Page_Is_Invisible(_pageKey);

		Show_Immediately_And_Check_Page_Is_Visible(_pageKey);

		Check_IsPassOnHidingStart<FakePage>(_pageKey, false);
		Check_IsPassPlayHidingAnimation<FakePage>(_pageKey, false);
		Check_IsPassPlayHidingAnimationImmediately<FakePage>(_pageKey, false);
		Check_IsPassOnHidingComplete<FakePage>(_pageKey, false);

		//act
		_pageContainer.HideAllImmediately();

		//assert
		Check_Page_Is_Invisible(_pageKey);

		Check_IsPassOnHidingStart<FakePage>(_pageKey, true);
		Check_IsPassPlayHidingAnimation<FakePage>(_pageKey, false);
		Check_IsPassPlayHidingAnimationImmediately<FakePage>(_pageKey, true);
		Check_IsPassOnHidingComplete<FakePage>(_pageKey, true);
	}

	[Test]
	public void _23_HideAllImmediately_With_Three_Step()
	{
		//arrange
		Create_And_Check_Page_Is_Created<FakePage>(_pageKey);
		Check_Page_Is_Invisible(_pageKey);

		Show_Immediately_And_Check_Page_Is_Visible(_pageKey);

		Check_IsPassOnHidingStart<FakePage>(_pageKey, false);
		Check_IsPassPlayHidingAnimation<FakePage>(_pageKey, false);
		Check_IsPassPlayHidingAnimationImmediately<FakePage>(_pageKey, false);
		Check_IsPassOnHidingComplete<FakePage>(_pageKey, false);

		//act
		_pageContainer.OnlyCallHidingStart(_pageKey);
		_pageContainer.HideAllImmediately(isIncludeHidingComplete: false);
		_pageContainer.OnlyCallAllHidingComplete();

		//assert
		Check_Page_Is_Invisible(_pageKey);

		Check_IsPassOnHidingStart<FakePage>(_pageKey, true);
		Check_IsPassPlayHidingAnimation<FakePage>(_pageKey, false);
		Check_IsPassPlayHidingAnimationImmediately<FakePage>(_pageKey, true);
		Check_IsPassOnHidingComplete<FakePage>(_pageKey, true);
	}

	[Test]
	public async void _24_Tick()
	{
		//arrange
		Create_And_Check_Page_Is_Created<FakePage>(_pageKey);
		Check_Page_Is_Invisible(_pageKey);

		await _pageContainer.ShowImmediatelyAsync(new[] { _pageKey });
		Check_Page_Is_Visible(_pageKey);

		Check_IsPassTick<FakePage>(_pageKey, false);

		//act
		_pageContainer.Tick(0);

		//assert
		Check_IsPassTick<FakePage>(_pageKey, true);
	}

	[Test]
	public async void _25_FixedTick()
	{
		//arrange
		Create_And_Check_Page_Is_Created<FakePage>(_pageKey);
		Check_Page_Is_Invisible(_pageKey);

		await _pageContainer.ShowImmediatelyAsync(new[] { _pageKey });
		Check_Page_Is_Visible(_pageKey);

		Check_IsPassFixedTick<FakePage>(_pageKey, false);

		//act
		_pageContainer.FixedTick(0);

		//assert
		Check_IsPassFixedTick<FakePage>(_pageKey, true);
	}

	//private method
	private void Check_Page_Is_Created<T>(string key) where T : MonoBehaviour =>
		Assert.IsTrue(_pageContainer.TryGetPage<T>(key, out var page), $"Page: {key} doesnt be created.");

	private void Create_And_Check_Page_Is_Created<T>(string key) where T : MonoBehaviour
	{
		_pageContainer.CreateAsync<T>(key);
		Check_Page_Is_Created<T>(key);
	}

	private void Check_Page_Is_Destroyed<T>(string key) where T : MonoBehaviour =>
		Assert.IsFalse(_pageContainer.TryGetPage<T>(key, out var page), $"Page: {key} doesnt be destroyed.");

	private void Check_Page_Is_Visible(string key) =>
		Assert.IsTrue(_pageContainer.CheckIsVisible(key), $"Page: {key} isnt visible.");

	private void Check_Page_Is_Invisible(string key) =>
		Assert.IsFalse(_pageContainer.CheckIsVisible(key), $"Page: {key} isnt invisible.");

	private async void Show_Immediately_And_Check_Page_Is_Visible(string key)
	{
		await _pageContainer.ShowImmediatelyAsync(key);
		Check_Page_Is_Visible(key);
	}

	private void Check_IsPassInitialize<T>(string key, bool excepted) where T : FakePage
	{
		Check_Page_Is_Created<T>(key);
		_pageContainer.TryGetPage<T>(key, out var page);
		Assert.AreEqual(excepted, page.IsPassInitialize, $"{key} IsPassInitialize doesnt match excepted: {excepted}.");
	}

	private void Check_IsPassTick<T>(string key, bool excepted) where T : FakePage
	{
		Check_Page_Is_Created<T>(key);
		_pageContainer.TryGetPage<T>(key, out var page);
		Assert.AreEqual(excepted, page.IsPassTick, $"{key} IsPassTick doesnt match excepted: {excepted}.");
	}

	private void Check_IsPassFixedTick<T>(string key, bool excepted) where T : FakePage
	{
		Check_Page_Is_Created<T>(key);
		_pageContainer.TryGetPage<T>(key, out var page);
		Assert.AreEqual(excepted, page.IsPassFixedTick, $"{key} IsPassFixedTick doesnt match excepted: {excepted}.");
	}

	private void Check_IsPassRelease(string key, bool excepted)
	{
		var gameObject = GameObject.Find(FakePage.IS_PASS_RELEASE_CREATE_GAME_OBJECT_NAME);
		Assert.AreEqual(excepted, gameObject != null, $"{key} IsPassRelease doesnt match excepted: {excepted}.");
	}

	private void Check_IsPassOnShowingStart<T>(string key, bool excepted) where T : FakePage
	{
		Check_Page_Is_Created<T>(key);
		_pageContainer.TryGetPage<T>(key, out var page);
		Assert.AreEqual(excepted, page.IsPassOnShowingStart, $"{key} IsPassOnShowingStart doesnt match excepted: {excepted}.");
	}

	private void Check_IsPassPlayShowingAnimation<T>(string key, bool excepted) where T : FakePage
	{
		Check_Page_Is_Created<T>(key);
		_pageContainer.TryGetPage<T>(key, out var page);
		Assert.AreEqual(excepted, page.IsPassPlayShowingAnimation, $"{key} IsPassPlayShowingAnimation doesnt match excepted: {excepted}.");
	}

	private void Check_IsPassPlayShowingAnimationImmediately<T>(string key, bool excepted) where T : FakePage
	{
		Check_Page_Is_Created<T>(key);
		_pageContainer.TryGetPage<T>(key, out var page);
		Assert.AreEqual(excepted, page.IsPassPlayShowingAnimationImmediately, $"{key} IsPassPlayShowingAnimationImmediately doesnt match excepted: {excepted}.");
	}

	private void Check_IsPassOnShowingComplete<T>(string key, bool excepted) where T : FakePage
	{
		Check_Page_Is_Created<T>(key);
		_pageContainer.TryGetPage<T>(key, out var page);
		Assert.AreEqual(excepted, page.IsPassOnShowingComplete, $"{key} IsPassOnShowingComplete doesnt match excepted: {excepted}.");
	}

	private void Check_IsPassOnHidingStart<T>(string key, bool excepted) where T : FakePage
	{
		Check_Page_Is_Created<T>(key);
		_pageContainer.TryGetPage<T>(key, out var page);
		Assert.AreEqual(excepted, page.IsPassOnHidingStart, $"{key} IsPassOnHidingStart doesnt match excepted: {excepted}.");
	}

	private void Check_IsPassPlayHidingAnimation<T>(string key, bool excepted) where T : FakePage
	{
		Check_Page_Is_Created<T>(key);
		_pageContainer.TryGetPage<T>(key, out var page);
		Assert.AreEqual(excepted, page.IsPassPlayHidingAnimation, $"{key} IsPassPlayHidingAnimation doesnt match excepted: {excepted}.");
	}

	private void Check_IsPassPlayHidingAnimationImmediately<T>(string key, bool excepted) where T : FakePage
	{
		Check_Page_Is_Created<T>(key);
		_pageContainer.TryGetPage<T>(key, out var page);
		Assert.AreEqual(excepted, page.IsPassPlayHidingAnimationImmediately, $"{key} IsPassPlayHidingAnimationImmediately doesnt match excepted: {excepted}.");
	}

	private void Check_IsPassOnHidingComplete<T>(string key, bool excepted) where T : FakePage
	{
		Check_Page_Is_Created<T>(key);
		_pageContainer.TryGetPage<T>(key, out var page);
		Assert.AreEqual(excepted, page.IsPassOnHidingComplete, $"{key} IsPassOnHidingComplete doesnt match excepted: {excepted}.");
	}

	private MonoBehaviour[] CreatePagePrefabs(Type[] pageTypes)
	{
		var pagePrefabs = new List<MonoBehaviour>();

		foreach (var pageType in pageTypes)
		{
			var page = CreatePagePrefab(pageType);
			pagePrefabs.Add(page);
		}

		return pagePrefabs.ToArray();
	}

	private MonoBehaviour CreatePagePrefab(Type pageType)
	{
		var prefab = new GameObject(pageType.Name);
		prefab.AddComponent(pageType);

		return prefab.GetComponent(pageType) as MonoBehaviour;
	}
}
