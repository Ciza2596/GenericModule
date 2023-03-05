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
        _pageContainer = new PageContainer(_pageGameObjectRootTransform, pagePrefabMap);
    }

    [TearDown]
    public void TearDown()
    {
        _pageContainer.DestroyAll();
        _pageContainer = null;

        var pageGameObjectRootTransform = _pageGameObjectRootTransform;
        _pageGameObjectRootTransform = null;
        Object.DestroyImmediate(pageGameObjectRootTransform.gameObject);
    }

    [Test]
    public void _01_Create()
    {
        //act
        _pageContainer.Create<FakePage>();

        //assert
        Check_Page_Is_Created<FakePage>();
        Check_IsInitializePass<FakePage>(true);
    }

    [Test]
    public void _02_CreateAll()
    {
        //act
        _pageContainer.CreateAll();

        //assert
        Check_Page_Is_Created<FakePage>();
        Check_IsInitializePass<FakePage>(true);
    }


    [Test]
    public void _03_Destroy()
    {
        //arrange
        Create_And_Check_Page_Is_Created<FakePage>();

        //act
        _pageContainer.Destroy<FakePage>();

        //assert
        Check_Page_Is_Destroyed<FakePage>();
    }

    [Test]
    public void _04_DestroyAll()
    {
        //arrange
        Create_And_Check_Page_Is_Created<FakePage>();

        //act
        _pageContainer.DestroyAll();

        //assert
        Check_Page_Is_Destroyed<FakePage>();
    }


    [Test]
    public async void _05_Show_A_Page()
    {
        //arrange
        Create_And_Check_Page_Is_Created<FakePage>();
        Check_Page_Is_Invisible<FakePage>();
        
        Check_IsBeforeShowingPass<FakePage>(false);
        Check_IsShowPass<FakePage>(false);
        Check_IsShowingActionPass<FakePage>(false);
        Check_IsCompleteShowingPass<FakePage>(false);

        //act
        await _pageContainer.Show<FakePage>();

        //assert
        Check_Page_Is_Visible<FakePage>();
        
        Check_IsBeforeShowingPass<FakePage>(true);
        Check_IsShowPass<FakePage>(true);
        Check_IsShowingActionPass<FakePage>(true);
        Check_IsCompleteShowingPass<FakePage>(true);
    }

    [Test]
    public async void _06_ShowImmediately_A_Page()
    {
        //arrange
        Create_And_Check_Page_Is_Created<FakePage>();
        Check_Page_Is_Invisible<FakePage>();
        
        Check_IsBeforeShowingPass<FakePage>(false);
        Check_IsShowPass<FakePage>(false);
        Check_IsShowingActionPass<FakePage>(false);
        Check_IsCompleteShowingPass<FakePage>(false);

        //act
        await _pageContainer.ShowImmediately<FakePage>();

        //assert
        Check_Page_Is_Visible<FakePage>();
        
        Check_IsBeforeShowingPass<FakePage>(true);
        Check_IsShowPass<FakePage>(true);
        Check_IsShowingActionPass<FakePage>(false);
        Check_IsCompleteShowingPass<FakePage>(true);
    }


    [Test]
    public async void _07_Show_Some_Pages()
    {
        //arrange
        Create_And_Check_Page_Is_Created<FakePage>();
        Check_Page_Is_Invisible<FakePage>();
        
        Check_IsBeforeShowingPass<FakePage>(false);
        Check_IsShowPass<FakePage>(false);
        Check_IsShowingActionPass<FakePage>(false);
        Check_IsCompleteShowingPass<FakePage>(false);

        //act
        await _pageContainer.Show(new[] { typeof(FakePage) }, new[] { Array.Empty<object>() });

        //assert
        Check_Page_Is_Visible<FakePage>();
        
        Check_IsBeforeShowingPass<FakePage>(true);
        Check_IsShowPass<FakePage>(true);
        Check_IsShowingActionPass<FakePage>(true);
        Check_IsCompleteShowingPass<FakePage>(true);
    }

    [Test]
    public async void _08_ShowImmediately_Some_Pages()
    {
        //arrange
        Create_And_Check_Page_Is_Created<FakePage>();
        Check_Page_Is_Invisible<FakePage>();

        Check_IsBeforeShowingPass<FakePage>(false);
        Check_IsShowPass<FakePage>(false);
        Check_IsShowingActionPass<FakePage>(false);
        Check_IsCompleteShowingPass<FakePage>(false);
        
        //act
        await _pageContainer.ShowImmediately(new[] { typeof(FakePage) }, new[] { Array.Empty<object>() });

        //assert
        Check_Page_Is_Visible<FakePage>();
        
        Check_IsBeforeShowingPass<FakePage>(true);
        Check_IsShowPass<FakePage>(true);
        Check_IsShowingActionPass<FakePage>(false);
        Check_IsCompleteShowingPass<FakePage>(true);
    }


    [Test]
    public async void _09_Hide_A_Page()
    {
        //arrange
        Create_And_Check_Page_Is_Created<FakePage>();
        Check_Page_Is_Invisible<FakePage>();

        Show_Immediately_And_Check_Page_Is_Visible<FakePage>();
        
        Check_IsHidePass<FakePage>(false);
        Check_IsHidingActionPass<FakePage>(false);
        Check_IsCompleteHidingPass<FakePage>(false);

        //act
        await _pageContainer.Hide<FakePage>();

        //assert
        Check_Page_Is_Invisible<FakePage>();
        
        Check_IsHidePass<FakePage>(true);
        Check_IsHidingActionPass<FakePage>(true);
        Check_IsCompleteHidingPass<FakePage>(true);
    }

    [Test]
    public void _10_HideImmediately_A_Page()
    {
        //arrange
        Create_And_Check_Page_Is_Created<FakePage>();
        Check_Page_Is_Invisible<FakePage>();

        Show_Immediately_And_Check_Page_Is_Visible<FakePage>();
        
        Check_IsHidePass<FakePage>(false);
        Check_IsHidingActionPass<FakePage>(false);
        Check_IsCompleteHidingPass<FakePage>(false);

        //act
        _pageContainer.HideImmediately<FakePage>();

        //assert
        Check_Page_Is_Invisible<FakePage>();
        
        Check_IsHidePass<FakePage>(true);
        Check_IsHidingActionPass<FakePage>(false);
        Check_IsCompleteHidingPass<FakePage>(true);
    }

    [Test]
    public async void _11_Hide_Some_Pages()
    {
        //arrange
        Create_And_Check_Page_Is_Created<FakePage>();
        Check_Page_Is_Invisible<FakePage>();

        Show_Immediately_And_Check_Page_Is_Visible<FakePage>();
        
        Check_IsHidePass<FakePage>(false);
        Check_IsHidingActionPass<FakePage>(false);
        Check_IsCompleteHidingPass<FakePage>(false);

        //act
        await _pageContainer.Hide(new[] { typeof(FakePage) });

        //assert
        Check_Page_Is_Invisible<FakePage>();
        
        Check_IsHidePass<FakePage>(true);
        Check_IsHidingActionPass<FakePage>(true);
        Check_IsCompleteHidingPass<FakePage>(true);
    }

    [Test]
    public void _12_HideImmediately_Some_Pages()
    {
        //arrange
        Create_And_Check_Page_Is_Created<FakePage>();
        Check_Page_Is_Invisible<FakePage>();

        Show_Immediately_And_Check_Page_Is_Visible<FakePage>();
        
        Check_IsHidePass<FakePage>(false);
        Check_IsHidingActionPass<FakePage>(false);
        Check_IsCompleteHidingPass<FakePage>(false);

        //act
        _pageContainer.HideImmediately(new[] { typeof(FakePage) });

        //assert
        Check_Page_Is_Invisible<FakePage>();
        
        Check_IsHidePass<FakePage>(true);
        Check_IsHidingActionPass<FakePage>(false);
        Check_IsCompleteHidingPass<FakePage>(true);
    }


    [Test]
    public async void _13_HideAll()
    {
        //arrange
        Create_And_Check_Page_Is_Created<FakePage>();
        Check_Page_Is_Invisible<FakePage>();

        Show_Immediately_And_Check_Page_Is_Visible<FakePage>();
        
        Check_IsHidePass<FakePage>(false);
        Check_IsHidingActionPass<FakePage>(false);
        Check_IsCompleteHidingPass<FakePage>(false);

        //act
        await _pageContainer.HideAll();

        //assert
        Check_Page_Is_Invisible<FakePage>();
        
        Check_IsHidePass<FakePage>(true);
        Check_IsHidingActionPass<FakePage>(true);
        Check_IsCompleteHidingPass<FakePage>(true);
    }

    [Test]
    public void _14_HideAllImmediately()
    {
        //arrange
        Create_And_Check_Page_Is_Created<FakePage>();
        Check_Page_Is_Invisible<FakePage>();

        Show_Immediately_And_Check_Page_Is_Visible<FakePage>();
        
        Check_IsHidePass<FakePage>(false);
        Check_IsHidingActionPass<FakePage>(false);
        Check_IsCompleteHidingPass<FakePage>(false);

        //act
        _pageContainer.HideAllImmediately();

        //assert
        Check_Page_Is_Invisible<FakePage>();
        
        Check_IsHidePass<FakePage>(true);
        Check_IsHidingActionPass<FakePage>(false);
        Check_IsCompleteHidingPass<FakePage>(true);
    }


    [Test]
    public async void _15_Update()
    {
        //arrange
        Create_And_Check_Page_Is_Created<FakePage>();
        Check_Page_Is_Invisible<FakePage>();
        
        await _pageContainer.ShowImmediately(new[] { typeof(FakePage) }, new[] { Array.Empty<object>() });
        Check_Page_Is_Visible<FakePage>();
        
        Check_IsOnUpdatePass<FakePage>(false);

        //act
        _pageContainer.Update(0);

        //assert
        Check_IsOnUpdatePass<FakePage>(true);
    }

    [Test]
    public async void _16_FixedUpdate()
    {
        //arrange
        Create_And_Check_Page_Is_Created<FakePage>();
        Check_Page_Is_Invisible<FakePage>();
        
        await _pageContainer.ShowImmediately(new[] { typeof(FakePage) }, new[] { Array.Empty<object>() });
        Check_Page_Is_Visible<FakePage>();
        
        Check_IsOnFixedUpdatePass<FakePage>(false);

        //act
        _pageContainer.FixedUpdate(0);

        //assert
        Check_IsOnFixedUpdatePass<FakePage>(true);
    }


    //private method
    private void Check_Page_Is_Created<T>() where T : Component
    {
        var pageType = typeof(T);
        Assert.IsTrue(_pageContainer.TryGetPage<T>(out var page), $"Page: {pageType} doesnt be created.");
    }

    private void Create_And_Check_Page_Is_Created<T>() where T : Component
    {
        _pageContainer.Create<T>();
        Check_Page_Is_Created<T>();
    }

    private void Check_Page_Is_Destroyed<T>() where T : Component
    {
        var pageType = typeof(T);
        Assert.IsFalse(_pageContainer.TryGetPage<T>(out var page), $"Page: {pageType} doesnt be destroyed.");
    }


    private void Check_Page_Is_Visible<T>() where T : Component
    {
        var pageType = typeof(T);
        Assert.IsTrue(_pageContainer.CheckIsVisible<T>(), $"Page: {pageType} isnt visible.");
    }

    private void Check_Page_Is_Invisible<T>() where T : Component
    {
        var pageType = typeof(T);
        Assert.IsFalse(_pageContainer.CheckIsVisible<T>(), $"Page: {pageType} isnt invisible.");
    }

    private async void Show_Immediately_And_Check_Page_Is_Visible<T>() where T : Component
    {
        await _pageContainer.ShowImmediately<T>();
        Check_Page_Is_Visible<T>();
    }


    private void Check_IsInitializePass<T>(bool excepted) where T : FakePage
    {
        Check_Page_Is_Created<T>();
        _pageContainer.TryGetPage<T>(out var page);
        Assert.AreEqual(excepted, page.IsInitializePass,
            $"{typeof(T)} IsInitializePass doesnt match excepted: {excepted}.");
    }

    private void Check_IsOnUpdatePass<T>(bool excepted) where T : FakePage
    {
        Check_Page_Is_Created<T>();
        _pageContainer.TryGetPage<T>(out var page);
        Assert.AreEqual(excepted, page.IsOnUpdatePass,
            $"{typeof(T)} IsOnUpdatePass doesnt match excepted: {excepted}.");
    }

    private void Check_IsOnFixedUpdatePass<T>(bool excepted) where T : FakePage
    {
        Check_Page_Is_Created<T>();
        _pageContainer.TryGetPage<T>(out var page);
        Assert.AreEqual(excepted, page.IsOnFixedUpdatePass,
            $"{typeof(T)} IsOnFixedUpdatePass doesnt match excepted: {excepted}.");
    }

    private void Check_IsReleasePass<T>(bool excepted) where T : FakePage
    {
        Check_Page_Is_Created<T>();
        _pageContainer.TryGetPage<T>(out var page);
        Assert.AreEqual(excepted, page.IsReleasePass, $"{typeof(T)} IsReleasePass doesnt match excepted: {excepted}.");
    }


    private void Check_IsBeforeShowingPass<T>(bool excepted) where T : FakePage
    {
        Check_Page_Is_Created<T>();
        _pageContainer.TryGetPage<T>(out var page);
        Assert.AreEqual(excepted, page.IsBeforeShowingPass,
            $"{typeof(T)} IsBeforeShowingPass doesnt match excepted: {excepted}.");
    }

    private void Check_IsShowPass<T>(bool excepted) where T : FakePage
    {
        Check_Page_Is_Created<T>();
        _pageContainer.TryGetPage<T>(out var page);
        Assert.AreEqual(excepted, page.IsShowPass, $"{typeof(T)} IsShowPass doesnt match excepted: {excepted}.");
    }

    private void Check_IsShowingActionPass<T>(bool excepted) where T : FakePage
    {
        Check_Page_Is_Created<T>();
        _pageContainer.TryGetPage<T>(out var page);
        Assert.AreEqual(excepted, page.IsShowingActionPass,
            $"{typeof(T)} IsShowingActionPass doesnt match excepted: {excepted}.");
    }

    private void Check_IsCompleteShowingPass<T>(bool excepted) where T : FakePage
    {
        Check_Page_Is_Created<T>();
        _pageContainer.TryGetPage<T>(out var page);
        Assert.AreEqual(excepted, page.IsCompleteShowingPass,
            $"{typeof(T)} IsCompleteShowingPass doesnt match excepted: {excepted}.");
    }


    private void Check_IsHidePass<T>(bool excepted) where T : FakePage
    {
        Check_Page_Is_Created<T>();
        _pageContainer.TryGetPage<T>(out var page);
        Assert.AreEqual(excepted, page.IsHidePass, $"{typeof(T)} IsHidePass doesnt match excepted: {excepted}.");
    }

    private void Check_IsHidingActionPass<T>(bool excepted) where T : FakePage
    {
        Check_Page_Is_Created<T>();
        _pageContainer.TryGetPage<T>(out var page);
        Assert.AreEqual(excepted, page.IsHidingActionPass,
            $"{typeof(T)} IsHidingActionPass doesnt match excepted: {excepted}.");
    }

    private void Check_IsCompleteHidingPass<T>(bool excepted) where T : FakePage
    {
        Check_Page_Is_Created<T>();
        _pageContainer.TryGetPage<T>(out var page);
        Assert.AreEqual(excepted, page.IsCompleteHidingPass,
            $"{typeof(T)} IsCompleteHidingPass doesnt match excepted: {excepted}.");
    }


    private Dictionary<Type, Component> CreatePagePrefabMap(Type[] pageTypes)
    {
        var pagePrefabMap = new Dictionary<Type, Component>();

        foreach (var pageType in pageTypes)
        {
            var page = CreatePagePrefabAndGetPage(pageType);
            pagePrefabMap.Add(pageType, page);
        }

        return pagePrefabMap;
    }

    private Component CreatePagePrefabAndGetPage(Type pageType)
    {
        var prefab = new GameObject(pageType.Name);
        prefab.AddComponent(pageType);

        return prefab.GetComponent(pageType);
    }
}