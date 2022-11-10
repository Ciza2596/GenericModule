using System;
using System.Collections.Generic;
using GameCore.Generic.Infrastructure;
using UnityEngine;
using UnityEngine.Assertions;
using Zenject;
using Object = UnityEngine.Object;


namespace ViewModule
{
    public class ViewModule: ITickable
    {
        //private variable
        private ITimeProvider _timeProvider;

        private Transform _viewParentTransform;
        private Dictionary<string, IView> _viewTemplates;


        private Dictionary<string, IView> _views = new Dictionary<string, IView>();

        private List<string> _currentShowingViewNames = new List<string>();
        private List<string> _currentVisibleViewNames = new List<string>();
        private List<string> _currentHidingViewNames = new List<string>();

        private List<string> _waitingReleaseViewNames = new List<string>();

        private Dictionary<string, Action> _onCompleteActions = new Dictionary<string, Action>();


        //zenject callback
        public void Tick()
        {
            var deltaTime = _timeProvider.GetDeltaTime();

            TickWaitingReleaseViews(_waitingReleaseViewNames.ToArray());
            TickCurrentShowingViews(_currentShowingViewNames.ToArray());
            TickCurrentHidingViews(_currentHidingViewNames.ToArray());
            TickCurrentVisibleViews(deltaTime, _currentVisibleViewNames.ToArray());
        }

        //public method 
        public ViewModule(ITimeProvider timeProvider, IViewDataOverview viewDataOverview)
        {
            _timeProvider = timeProvider;
            _viewTemplates = viewDataOverview.GetViewTemplates();
            
            var viewParentTransformName = viewDataOverview.GetViewParentTransformName();
            var viewParent = new GameObject(viewParentTransformName);
            _viewParentTransform = viewParent.transform;
        }


        public T GetViewComponent<T>(string viewName) => _views[viewName].GameObject.GetComponent<T>();
        public bool GetIsVisibleView(string viewName) => _currentVisibleViewNames.Contains(viewName);
        public bool GetIsShowing(string viewName) => _views[viewName].IsShowing;
        public bool GetIsHiding(string viewName) => _views[viewName].IsHiding;


        public void LoadView(string viewName, params object[] parameters)
        {
            Assert.IsTrue(_viewTemplates.ContainsKey(viewName),
                          $"[ViewModule::LoadView] ViewTemplates hasn't viewTemplate: {viewName}.");
            Assert.IsTrue(!_views.ContainsKey(viewName), $"[ViewModule::LoadView] View exists. ViewName: {viewName}.");
            
            
            var viewTemplate = _viewTemplates[viewName];
            var viewGameObject = Object.Instantiate(viewTemplate.GameObject, _viewParentTransform);
            var view = viewGameObject.GetComponent<IView>();
            view.Init(parameters);

            _views.Add(viewName, view);
        }

        public void LoadAllViews()
        {
            foreach (var template in _viewTemplates)
            {
                var viewName = template.Key;
                LoadView(viewName);
            }
        }

        public void ReleaseView(string viewName)
        {
            Assert.IsTrue(_views.ContainsKey(viewName), $"[ViewModule::ReleaseView] Views hasn't view: {viewName}.");

            if (_currentVisibleViewNames.Contains(viewName))
                HideView(viewName);

            _waitingReleaseViewNames.Add(viewName);
        }

        public void ReleaseAllViews()
        {
            foreach (var template in _viewTemplates)
            {
                var viewName = template.Key;
                ReleaseView(viewName);
            }
        }

        public void ShowView(string viewName, params object[] parameters)
        {
            if (_currentShowingViewNames.Contains(viewName) || _currentVisibleViewNames.Contains(viewName) ||
                _currentHidingViewNames.Contains(viewName))
            {
                Debug.LogWarning($"[ViewModule::ShowView] View: {viewName} show fail.");
                return;
            }

            _currentVisibleViewNames.Add(viewName);
            _currentShowingViewNames.Add(viewName);

            var view = _views[viewName];
            view.Show(parameters);
        }

        public void HideView(string viewName, Action onCompleteAction = null)
        {
            if (_currentHidingViewNames.Contains(viewName) || !_currentVisibleViewNames.Contains(viewName) ||
                _currentShowingViewNames.Contains(viewName))
            {
                Debug.LogWarning($"[ViewModule::HideView] View: {viewName} hide fail.");
                return;
            }

            if (onCompleteAction != null)
                _onCompleteActions.Add(viewName, onCompleteAction);
            

            _currentHidingViewNames.Add(viewName);

            var view = _views[viewName];
            view.Hide();
        }


        //private method
        private void TickWaitingReleaseViews(string[] waitingReleaseViewNames)
        {
            foreach (var waitingReleaseViewName in waitingReleaseViewNames)
            {
                if (_currentHidingViewNames.Contains(waitingReleaseViewName))
                    continue;

                _waitingReleaseViewNames.Remove(waitingReleaseViewName);

                var view = _views[waitingReleaseViewName];
                view.Release();
                Object.DestroyImmediate(view.GameObject);
            }
        }

        private void TickCurrentShowingViews(string[] currentShowingViewNames)
        {
            foreach (var isShowingViewName in currentShowingViewNames)
            {
                var view = _views[isShowingViewName];

                if (view.IsShowing)
                    continue;

                _currentShowingViewNames.Remove(isShowingViewName);

                _currentVisibleViewNames.Add(isShowingViewName);
            }
        }

        private void TickCurrentHidingViews(string[] currentHidingViewNames)
        {
            foreach (var isHidingViewName in currentHidingViewNames)
            {
                var view = _views[isHidingViewName];

                if (view.IsHiding)
                    continue;

                view.HideAfter();
                _currentHidingViewNames.Remove(isHidingViewName);
                _currentVisibleViewNames.Remove(isHidingViewName);
                _currentVisibleViewNames.Remove(isHidingViewName);

                if (_onCompleteActions.ContainsKey(isHidingViewName))
                {
                    var onCompletedAction = _onCompleteActions[isHidingViewName];
                    _onCompleteActions.Remove(isHidingViewName);
                    onCompletedAction();
                }
            }
        }

        private void TickCurrentVisibleViews(float deltaTime, string[] currentVisibleViews)
        {
            foreach (var canUpdateViewName in currentVisibleViews)
            {
                if (!_views.ContainsKey(canUpdateViewName))
                    continue;

                var view = _views[canUpdateViewName];
                view.Tick(deltaTime);
            }
        }
    }
}