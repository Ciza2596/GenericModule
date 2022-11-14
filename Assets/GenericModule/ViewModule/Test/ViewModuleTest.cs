using System.Collections.Generic;
using GameCore.Generic.Infrastructure;
using GameCore.Generic.TestFrameWork;
using NSubstitute;
using NUnit.Framework;
using UnityEngine;
using ViewModule;
using ViewModule.Derived;

public sealed class ViewModuleTest: DIUnitTestFixture
{
    private ViewModule.ViewModule _viewModule;
    private IViewDataOverview _viewDataOverview;
    private Dictionary<string, IView> _viewTemplates;

    private string _viewParentTransformName = "ViewParent";
    private string _viewName_01 = "ViewImp_01";
    private string _viewName_02 = "ViewImp_02";
    
    
    [SetUp]
    public override void Setup()
    {
        base.Setup();
        
        var timeProvider = Substitute.For<ITimeProvider>();
        Container.Bind<ITimeProvider>().FromInstance(timeProvider);

        
        _viewDataOverview = Substitute.For<IViewDataOverview>();
        
        _viewTemplates = new Dictionary<string, IView>();
        
        var view_01 = Substitute.For<IView>();
        var viewPrefab_01 = new GameObject(_viewName_01);
        viewPrefab_01.AddComponent<ViewImp>();
        view_01.GameObject.Returns(viewPrefab_01);
        _viewTemplates.Add(_viewName_01, view_01);
        
        var view_02 = Substitute.For<IView>();
        var viewPrefab_02 = new GameObject(_viewName_02);
        viewPrefab_02.AddComponent<ViewImp>();
        view_02.GameObject.Returns(viewPrefab_02);
        _viewTemplates.Add(_viewName_02, view_02);

        _viewDataOverview.GetViewParentTransformName().Returns(_viewParentTransformName);
        _viewDataOverview.GetViewTemplates().Returns(_viewTemplates);
        
        
        Container.Bind<IViewDataOverview>().FromInstance(_viewDataOverview);
        
        Container.BindInterfacesAndSelfTo<ViewModule.ViewModule>().AsSingle();
        _viewModule = Container.Resolve<ViewModule.ViewModule>();
    }

    [Test]
    public void Should_Success_When_LoadView()
    {
        //arrange
        Assert.IsTrue(_viewModule.Views.Count == 0, "Not init.");
        
        
        //act
        var expectViewParameter = "Hello";
        _viewModule.LoadView(_viewName_01, expectViewParameter);
        
        
        //assert
        var view = _viewModule.GetViewComponent<IView>(_viewName_01);
        Assert.IsNotNull(view, $"Not load view.");
        
        var viewImp = _viewModule.GetViewComponent<ViewImp>(_viewName_01);
        var initParameters = viewImp.GetInitParameters();
        var viewParameter = initParameters[0] as string;
        Assert.IsTrue(viewParameter == expectViewParameter, "View show fail.");
    }

    [Test]
    public void Should_Success_When_ReleaseView()
    {
        //arrange
        _viewModule.LoadView(_viewName_01);
        Assert.IsTrue(_viewModule.Views.Count == 1, "Not init.");

        
        //act
        _viewModule.ReleaseView(_viewName_01);
        _viewModule.Tick();
        
        
        //assert
        Assert.IsTrue(_viewModule.Views.Count == 0, "Not release view.");
    }

    [Test]
    public void Should_Success_When_ReleaseAllViews()
    {
        //arrange
        _viewModule.LoadView(_viewName_01);
        _viewModule.LoadView(_viewName_02);
        Assert.IsTrue(_viewModule.Views.Count == 2, "Not init.");
        
        
        //act
        _viewModule.ReleaseAllViews();
        _viewModule.Tick();
        
        
        //assert
        Assert.IsTrue(_viewModule.Views.Count == 0, "Not release view.");
    }

    [Test]
    public void Should_Success_When_ShowView()
    {
        //arrange
        _viewModule.LoadView(_viewName_01);
        Assert.IsTrue(_viewModule.Views.Count == 1, "Not init.");
        
        var isVisible = _viewModule.GetIsVisible(_viewName_01);
        Assert.IsFalse(isVisible, "Not init");


        //act
        var expectViewParameter = "Hello";
        _viewModule.ShowView(_viewName_01, expectViewParameter);
        _viewModule.Tick();
        
        
        //assert
        var viewImp = _viewModule.GetViewComponent<ViewImp>(_viewName_01);
        var showParameters = viewImp.GetShowParameters();
        var viewParameter = showParameters[0] as string;
        Assert.IsTrue(viewParameter == expectViewParameter, "View show fail.");

        isVisible = _viewModule.GetIsVisible(_viewName_01);
        Assert.IsTrue(isVisible, "View show fail.");
    }
    
    [Test]
    public void Should_Success_When_HideView()
    {
        //arrange
        _viewModule.LoadView(_viewName_01);
        Assert.IsTrue(_viewModule.Views.Count == 1, "Not init.");
        
        _viewModule.ShowView(_viewName_01);
        _viewModule.Tick();
        
        var isVisible = _viewModule.GetIsVisible(_viewName_01);
        Assert.IsTrue(isVisible, "Not init");


        //act
        _viewModule.HideView(_viewName_01);
        _viewModule.Tick();
        
        
        //assert
        isVisible = _viewModule.GetIsVisible(_viewName_01);
        Assert.IsFalse(isVisible, "View hide fail.");
    }
}