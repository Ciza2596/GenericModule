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
    }

    [Test]
    public void _02_CreateAll()
    {
        //act
        _pageContainer.CreateAll();

        //assert
        Check_Page_Is_Created<FakePage>();
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

        return prefab.GetComponent(pageType) as Component;
    }
}