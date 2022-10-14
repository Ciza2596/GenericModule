using System.Collections.Generic;
using GameCore.Exposed;
using NSubstitute;
using NUnit.Framework;
using UnityEngine;

namespace ViewModule.Test
{
    public sealed class ViewModuleTest
    {
        private ViewModule _viewModule;

        private IView _view;
        private const string EXAMPLE_VIEW_NAME = "KK";

        [SetUp]
        public void SetUp()
        {
            var viewPrefab = new GameObject();
            var viewForTest = viewPrefab.AddComponent<ExampleView>();
            viewForTest.SetViewName(EXAMPLE_VIEW_NAME);

            List<GameObject> viewPrefabs = new List<GameObject>() { viewPrefab };

            var viewDataOverview = ScriptableObject.CreateInstance<ViewDataOverview>();
            viewDataOverview.InitViewDataOverview(viewPrefabs);

            var logModule = new LogModule();
            _viewModule = new ViewModule();
            _viewModule.Initialize(logModule, viewDataOverview);

            _view = Substitute.For<IView>();
        }

        [TearDown]
        public void TearDown()
        {
            _viewModule = null;
            _view = null;
        }

        [Test]
        public void Should_Success_When_RegisterView()
        {
            //arrange
            Assert.AreEqual(_viewModule.Views.Count, 0);

            //act
            _viewModule.RegisterView(_view);

            //assert
            Assert.AreEqual(_viewModule.Views.Count, 1);
        }

        [Test]
        public void Should_Success_When_LoadView()
        {
            //arrange
            Assert.AreEqual(_viewModule.Views.Count, 0);

            //act
            _viewModule.LoadView(EXAMPLE_VIEW_NAME);

            //assert
            Assert.AreEqual(_viewModule.Views.Count, 1);
        }

        [Test]
        public void Should_Success_When_ReleaseView()
        {
            //arrange
            _viewModule.LoadView(EXAMPLE_VIEW_NAME);
            Assert.AreEqual(_viewModule.Views.Count, 1);

            //act
            _viewModule.ReleaseView(EXAMPLE_VIEW_NAME);

            //assert
            Assert.AreEqual(_viewModule.Views.Count, 0);
        }

        [Test]
        public void Should_Success_When_GetHasView()
        {
            //arrange
            _viewModule.LoadView(EXAMPLE_VIEW_NAME);
            Assert.AreEqual(_viewModule.Views.Count, 1);

            //act
            var hasView = _viewModule.GetHasView(EXAMPLE_VIEW_NAME);

            //assert
            Assert.AreEqual(hasView, true);
        }

        [Test]
        [TestCase(EXAMPLE_VIEW_NAME, true)]
        public void Should_Success_When_ShowView(string viewName, bool isImmediately)
        {
            
            
            //arrange
            _view.Name.Returns(viewName);
            _viewModule.RegisterView(_view);
            Assert.AreEqual(_viewModule.Views.Count, 1);
            Assert.AreEqual(_viewModule.VisibleViews.Count, 0);

            //act
            _viewModule.ShowView(viewName, isImmediately);

            //assert
            Assert.AreEqual(_viewModule.VisibleViews.Count, 1);
        }


        [TestCase(EXAMPLE_VIEW_NAME, false, true)]
        [TestCase(EXAMPLE_VIEW_NAME, true, false)]
        public void Should_Success_When_ShowViewAsync(string viewName, bool isShowing, bool finalIsOpen)
        {
            //arrange
            _view.Name.Returns(viewName);
            _view.IsShowing.Returns(isShowing);

            _viewModule.RegisterView(_view);
            Assert.AreEqual(_viewModule.Views.Count, 1);

            _viewModule.HideView(viewName, true);
            Assert.AreEqual(_viewModule.VisibleViews.Count, 0);

            //act
            _viewModule.ShowViewAsync(viewName);

            //assert
            var openedViewNumber = finalIsOpen ? 1 : 0;
            Assert.AreEqual(_viewModule.VisibleViews.Count, openedViewNumber);
        }


        [Test]
        [TestCase(EXAMPLE_VIEW_NAME, true)]
        public void Should_Success_When_HideView(string viewName, bool isImmediately)
        {
            //arrange
            _view.Name.Returns(viewName);
            _viewModule.RegisterView(_view);
            Assert.AreEqual(_viewModule.Views.Count, 1);

            _viewModule.ShowView(viewName, isImmediately);
            Assert.AreEqual(_viewModule.VisibleViews.Count, 1);

            //act
            _viewModule.HideView(viewName, isImmediately);

            //assert
            Assert.AreEqual(_viewModule.VisibleViews.Count, 0);
        }


        [TestCase(EXAMPLE_VIEW_NAME, false, true)]
        [TestCase(EXAMPLE_VIEW_NAME, true, false)]
        public void Should_Success_When_HideViewAsync(string viewName, bool isHiding, bool finalIsHide)
        {
            //arrange
            _view.Name.Returns(viewName);
            _view.IsHiding.Returns(isHiding);

            _viewModule.RegisterView(_view);
            Assert.AreEqual(_viewModule.Views.Count, 1);

            _viewModule.ShowView(viewName, true);
            Assert.AreEqual(_viewModule.VisibleViews.Count, 1);

            //act
            _viewModule.HideViewAsync(viewName);

            //assert
            var openedViewNumber = finalIsHide ? 0 : 1;
            Assert.AreEqual(_viewModule.VisibleViews.Count, openedViewNumber);
        }


        [Test]
        [TestCase(EXAMPLE_VIEW_NAME, true)]
        public void Should_Success_When_HideAllView(string viewName, bool isImmediately)
        {
            //arrange
            _view.Name.Returns(EXAMPLE_VIEW_NAME);
            _viewModule.RegisterView(_view);
            Assert.AreEqual(_viewModule.Views.Count, 1);

            _viewModule.ShowView(EXAMPLE_VIEW_NAME, isImmediately);
            Assert.AreEqual(_viewModule.VisibleViews.Count, 1);

            //act
            _viewModule.HideAllView(isImmediately);

            //assert
            Assert.AreEqual(_viewModule.VisibleViews.Count, 0);
        }
    }
}