using System;
using System.Collections.Generic;
using NUnit.Framework;
using PageModule;
using UnityEngine;
using Object = UnityEngine.Object;

public class PageContainerTest
{
    private PageContainer _pageContainer;
    private Transform _pageGameObjectRootTransform;

    [SetUp]
    public void SetUp()
    {
        var pageGameObjectRoot = new GameObject();
        _pageGameObjectRootTransform = pageGameObjectRoot.transform;
        var pagePrefabMap = CreatePagePrefabMap(new[]
        {
            typeof(FakePage)
        });
        _pageContainer = new PageContainer();
        _pageContainer.Initialize(_pageGameObjectRootTransform, pagePrefabMap);
    }

    [TearDown]
    public void TearDown()
    {
        _pageContainer.DestroyAll();
        _pageContainer = null;

        var pageGameObjectRootTransform = _pageGameObjectRootTransform;
        _pageGameObjectRootTransform = null;
        Object.DestroyImmediate(pageGameObjectRootTransform.gameObject);

        var gameObject = GameObject.Find(FakePage.IS_PASS_RELEASE_CREATE_GAMEOBJECT_NAME);
        if (gameObject != null)
            Object.DestroyImmediate(gameObject);
    }

    [Test]
    public void _01_Create()
    {
        //act
        _pageContainer.Create<FakePage>();

        //assert
        Check_Page_Is_Created<FakePage>();
        Check_IsPassInitialize<FakePage>(true);
    }

    [Test]
    public void _02_CreateAll()
    {
        //act
        _pageContainer.CreateAll();

        //assert
        Check_Page_Is_Created<FakePage>();
        Check_IsPassInitialize<FakePage>(true);
    }


    [Test]
    public void _03_Destroy()
    {
        //arrange
        Create_And_Check_Page_Is_Created<FakePage>();
        Check_IsPassRelease<FakePage>(false);

        //act
        _pageContainer.Destroy<FakePage>();

        //assert
        Check_Page_Is_Destroyed<FakePage>();
        Check_IsPassRelease<FakePage>(true);
    }

    [Test]
    public void _04_DestroyAll()
    {
        //arrange
        Create_And_Check_Page_Is_Created<FakePage>();
        Check_IsPassRelease<FakePage>(false);

        //act
        _pageContainer.DestroyAll();

        //assert
        Check_Page_Is_Destroyed<FakePage>();
        Check_IsPassRelease<FakePage>(true);
    }


    [Test]
    public async void _05_Show_A_Page()
    {
        //arrange
        Create_And_Check_Page_Is_Created<FakePage>();
        Check_Page_Is_Invisible<FakePage>();

        Check_IsPassOnShowingStart<FakePage>(false);
        Check_IsPassPlayShowingAnimation<FakePage>(false);
        Check_IsPassOnShowingComplete<FakePage>(false);

        //act
        await _pageContainer.Show<FakePage>();

        //assert
        Check_Page_Is_Visible<FakePage>();

        Check_IsPassOnShowingStart<FakePage>(true);
        Check_IsPassPlayShowingAnimation<FakePage>(true);
        Check_IsPassOnShowingComplete<FakePage>(true);
    }

    [Test]
    public async void _06_ShowImmediately_A_Page()
    {
        //arrange
        Create_And_Check_Page_Is_Created<FakePage>();
        Check_Page_Is_Invisible<FakePage>();

        Check_IsPassOnShowingStart<FakePage>(false);
        Check_IsPassPlayShowingAnimation<FakePage>(false);
        Check_IsPassOnShowingComplete<FakePage>(false);

        //act
        await _pageContainer.ShowImmediately<FakePage>();

        //assert
        Check_Page_Is_Visible<FakePage>();

        Check_IsPassOnShowingStart<FakePage>(true);
        Check_IsPassPlayShowingAnimation<FakePage>(false);
        Check_IsPassOnShowingComplete<FakePage>(true);
    }


    [Test]
    public async void _07_Show_Some_Pages()
    {
        //arrange
        Create_And_Check_Page_Is_Created<FakePage>();
        Check_Page_Is_Invisible<FakePage>();

        Check_IsPassOnShowingStart<FakePage>(false);
        Check_IsPassPlayShowingAnimation<FakePage>(false);
        Check_IsPassOnShowingComplete<FakePage>(false);

        //act
        await _pageContainer.Show(new[] { typeof(FakePage) });

        //assert
        Check_Page_Is_Visible<FakePage>();

        Check_IsPassOnShowingStart<FakePage>(true);
        Check_IsPassPlayShowingAnimation<FakePage>(true);
        Check_IsPassOnShowingComplete<FakePage>(true);
    }

    [Test]
    public async void _08_ShowImmediately_Some_Pages()
    {
        //arrange
        Create_And_Check_Page_Is_Created<FakePage>();
        Check_Page_Is_Invisible<FakePage>();

        Check_IsPassOnShowingStart<FakePage>(false);
        Check_IsPassPlayShowingAnimation<FakePage>(false);
        Check_IsPassOnShowingComplete<FakePage>(false);

        //act
        await _pageContainer.ShowImmediately(new[] { typeof(FakePage) });

        //assert
        Check_Page_Is_Visible<FakePage>();

        Check_IsPassOnShowingStart<FakePage>(true);
        Check_IsPassPlayShowingAnimation<FakePage>(false);
        Check_IsPassOnShowingComplete<FakePage>(true);
    }


    [Test]
    public async void _09_Hide_A_Page()
    {
        //arrange
        Create_And_Check_Page_Is_Created<FakePage>();
        Check_Page_Is_Invisible<FakePage>();

        Show_Immediately_And_Check_Page_Is_Visible<FakePage>();

        Check_IsPassOnHidingStart<FakePage>(false);
        Check_IsPassPlayHidingAnimation<FakePage>(false);
        Check_IsPassOnHidingComplete<FakePage>(false);

        //act
        await _pageContainer.Hide<FakePage>();

        //assert
        Check_Page_Is_Invisible<FakePage>();

        Check_IsPassOnHidingStart<FakePage>(true);
        Check_IsPassPlayHidingAnimation<FakePage>(true);
        Check_IsPassOnHidingComplete<FakePage>(true);
    }

    [Test]
    public void _10_HideImmediately_A_Page()
    {
        //arrange
        Create_And_Check_Page_Is_Created<FakePage>();
        Check_Page_Is_Invisible<FakePage>();

        Show_Immediately_And_Check_Page_Is_Visible<FakePage>();

        Check_IsPassOnHidingStart<FakePage>(false);
        Check_IsPassPlayHidingAnimation<FakePage>(false);
        Check_IsPassOnHidingComplete<FakePage>(false);

        //act
        _pageContainer.HideImmediately<FakePage>();

        //assert
        Check_Page_Is_Invisible<FakePage>();

        Check_IsPassOnHidingStart<FakePage>(true);
        Check_IsPassPlayHidingAnimation<FakePage>(false);
        Check_IsPassOnHidingComplete<FakePage>(true);
    }

    [Test]
    public async void _11_Hide_Some_Pages()
    {
        //arrange
        Create_And_Check_Page_Is_Created<FakePage>();
        Check_Page_Is_Invisible<FakePage>();

        Show_Immediately_And_Check_Page_Is_Visible<FakePage>();

        Check_IsPassOnHidingStart<FakePage>(false);
        Check_IsPassPlayHidingAnimation<FakePage>(false);
        Check_IsPassOnHidingComplete<FakePage>(false);

        //act
        await _pageContainer.Hide(new[] { typeof(FakePage) });

        //assert
        Check_Page_Is_Invisible<FakePage>();

        Check_IsPassOnHidingStart<FakePage>(true);
        Check_IsPassPlayHidingAnimation<FakePage>(true);
        Check_IsPassOnHidingComplete<FakePage>(true);
    }

    [Test]
    public void _12_HideImmediately_Some_Pages()
    {
        //arrange
        Create_And_Check_Page_Is_Created<FakePage>();
        Check_Page_Is_Invisible<FakePage>();

        Show_Immediately_And_Check_Page_Is_Visible<FakePage>();

        Check_IsPassOnHidingStart<FakePage>(false);
        Check_IsPassPlayHidingAnimation<FakePage>(false);
        Check_IsPassOnHidingComplete<FakePage>(false);

        //act
        _pageContainer.HideImmediately(new[] { typeof(FakePage) });

        //assert
        Check_Page_Is_Invisible<FakePage>();

        Check_IsPassOnHidingStart<FakePage>(true);
        Check_IsPassPlayHidingAnimation<FakePage>(false);
        Check_IsPassOnHidingComplete<FakePage>(true);
    }


    [Test]
    public async void _13_HideAll()
    {
        //arrange
        Create_And_Check_Page_Is_Created<FakePage>();
        Check_Page_Is_Invisible<FakePage>();

        Show_Immediately_And_Check_Page_Is_Visible<FakePage>();

        Check_IsPassOnHidingStart<FakePage>(false);
        Check_IsPassPlayHidingAnimation<FakePage>(false);
        Check_IsPassOnHidingComplete<FakePage>(false);

        //act
        await _pageContainer.HideAll();

        //assert
        Check_Page_Is_Invisible<FakePage>();

        Check_IsPassOnHidingStart<FakePage>(true);
        Check_IsPassPlayHidingAnimation<FakePage>(true);
        Check_IsPassOnHidingComplete<FakePage>(true);
    }

    [Test]
    public void _14_HideAllImmediately()
    {
        //arrange
        Create_And_Check_Page_Is_Created<FakePage>();
        Check_Page_Is_Invisible<FakePage>();

        Show_Immediately_And_Check_Page_Is_Visible<FakePage>();

        Check_IsPassOnHidingStart<FakePage>(false);
        Check_IsPassPlayHidingAnimation<FakePage>(false);
        Check_IsPassOnHidingComplete<FakePage>(false);

        //act
        _pageContainer.HideAllImmediately();

        //assert
        Check_Page_Is_Invisible<FakePage>();

        Check_IsPassOnHidingStart<FakePage>(true);
        Check_IsPassPlayHidingAnimation<FakePage>(false);
        Check_IsPassOnHidingComplete<FakePage>(true);
    }


    [Test]
    public async void _15_Tick()
    {
        //arrange
        Create_And_Check_Page_Is_Created<FakePage>();
        Check_Page_Is_Invisible<FakePage>();

        await _pageContainer.ShowImmediately(new[] { typeof(FakePage) }, new[] { Array.Empty<object>() });
        Check_Page_Is_Visible<FakePage>();

        Check_IsPassTick<FakePage>(false);

        //act
        _pageContainer.Tick(0);

        //assert
        Check_IsPassTick<FakePage>(true);
    }

    [Test]
    public async void _16_FixedTick()
    {
        //arrange
        Create_And_Check_Page_Is_Created<FakePage>();
        Check_Page_Is_Invisible<FakePage>();

        await _pageContainer.ShowImmediately(new[] { typeof(FakePage) }, new[] { Array.Empty<object>() });
        Check_Page_Is_Visible<FakePage>();

        Check_IsPassFixedTick<FakePage>(false);

        //act
        _pageContainer.FixedTick(0);

        //assert
        Check_IsPassFixedTick<FakePage>(true);
    }


    //private method
    private void Check_Page_Is_Created<T>() where T : MonoBehaviour
    {
        var pageType = typeof(T);
        Assert.IsTrue(_pageContainer.TryGetPage<T>(out var page), $"Page: {pageType} doesnt be created.");
    }

    private void Create_And_Check_Page_Is_Created<T>() where T : MonoBehaviour
    {
        _pageContainer.Create<T>();
        Check_Page_Is_Created<T>();
    }

    private void Check_Page_Is_Destroyed<T>() where T : MonoBehaviour
    {
        var pageType = typeof(T);
        Assert.IsFalse(_pageContainer.TryGetPage<T>(out var page), $"Page: {pageType} doesnt be destroyed.");
    }


    private void Check_Page_Is_Visible<T>() where T : MonoBehaviour
    {
        var pageType = typeof(T);
        Assert.IsTrue(_pageContainer.CheckIsVisible<T>(), $"Page: {pageType} isnt visible.");
    }

    private void Check_Page_Is_Invisible<T>() where T : MonoBehaviour
    {
        var pageType = typeof(T);
        Assert.IsFalse(_pageContainer.CheckIsVisible<T>(), $"Page: {pageType} isnt invisible.");
    }

    private async void Show_Immediately_And_Check_Page_Is_Visible<T>() where T : MonoBehaviour
    {
        await _pageContainer.ShowImmediately<T>();
        Check_Page_Is_Visible<T>();
    }


    private void Check_IsPassInitialize<T>(bool excepted) where T : FakePage
    {
        Check_Page_Is_Created<T>();
        _pageContainer.TryGetPage<T>(out var page);
        Assert.AreEqual(excepted, page.IsPassInitialize,
            $"{typeof(T)} IsPassInitialize doesnt match excepted: {excepted}.");
    }

    private void Check_IsPassTick<T>(bool excepted) where T : FakePage
    {
        Check_Page_Is_Created<T>();
        _pageContainer.TryGetPage<T>(out var page);
        Assert.AreEqual(excepted, page.IsPassTick,
            $"{typeof(T)} IsPassTick doesnt match excepted: {excepted}.");
    }

    private void Check_IsPassFixedTick<T>(bool excepted) where T : FakePage
    {
        Check_Page_Is_Created<T>();
        _pageContainer.TryGetPage<T>(out var page);
        Assert.AreEqual(excepted, page.IsPassFixedTick,
            $"{typeof(T)} IsPassFixedTick doesnt match excepted: {excepted}.");
    }

    private void Check_IsPassRelease<T>(bool excepted) where T : FakePage
    {
        var gameObject = GameObject.Find(FakePage.IS_PASS_RELEASE_CREATE_GAMEOBJECT_NAME);
        Assert.AreEqual(excepted, gameObject != null, $"{typeof(T)} IsPassRelease doesnt match excepted: {excepted}.");
    }


    private void Check_IsPassOnShowingStart<T>(bool excepted) where T : FakePage
    {
        Check_Page_Is_Created<T>();
        _pageContainer.TryGetPage<T>(out var page);
        Assert.AreEqual(excepted, page.IsPassOnShowingStart,
            $"{typeof(T)} IsPassOnShowingStart doesnt match excepted: {excepted}.");
    }


    private void Check_IsPassPlayShowingAnimation<T>(bool excepted) where T : FakePage
    {
        Check_Page_Is_Created<T>();
        _pageContainer.TryGetPage<T>(out var page);
        Assert.AreEqual(excepted, page.IsPassPlayShowingAnimation,
            $"{typeof(T)} IsPassPlayShowingAnimation doesnt match excepted: {excepted}.");
    }

    private void Check_IsPassOnShowingComplete<T>(bool excepted) where T : FakePage
    {
        Check_Page_Is_Created<T>();
        _pageContainer.TryGetPage<T>(out var page);
        Assert.AreEqual(excepted, page.IsPassOnShowingComplete,
            $"{typeof(T)} IsPassOnShowingComplete doesnt match excepted: {excepted}.");
    }


    private void Check_IsPassOnHidingStart<T>(bool excepted) where T : FakePage
    {
        Check_Page_Is_Created<T>();
        _pageContainer.TryGetPage<T>(out var page);
        Assert.AreEqual(excepted, page.IsPassOnHidingStart,
            $"{typeof(T)} IsPassOnHidingStart doesnt match excepted: {excepted}.");
    }

    private void Check_IsPassPlayHidingAnimation<T>(bool excepted) where T : FakePage
    {
        Check_Page_Is_Created<T>();
        _pageContainer.TryGetPage<T>(out var page);
        Assert.AreEqual(excepted, page.IsPassPlayHidingAnimation,
            $"{typeof(T)} IsPassPlayHidingAnimation doesnt match excepted: {excepted}.");
    }

    private void Check_IsPassOnHidingComplete<T>(bool excepted) where T : FakePage
    {
        Check_Page_Is_Created<T>();
        _pageContainer.TryGetPage<T>(out var page);
        Assert.AreEqual(excepted, page.IsPassOnHidingComplete,
            $"{typeof(T)} IsPassOnHidingComplete doesnt match excepted: {excepted}.");
    }


    private Dictionary<Type, MonoBehaviour> CreatePagePrefabMap(Type[] pageTypes)
    {
        var pagePrefabMap = new Dictionary<Type, MonoBehaviour>();

        foreach (var pageType in pageTypes)
        {
            var page = CreatePagePrefabAndGetPage(pageType);
            pagePrefabMap.Add(pageType, page);
        }

        return pagePrefabMap;
    }

    private MonoBehaviour CreatePagePrefabAndGetPage(Type pageType)
    {
        var prefab = new GameObject(pageType.Name);
        prefab.AddComponent(pageType);

        return prefab.GetComponent(pageType) as MonoBehaviour;
    }
}