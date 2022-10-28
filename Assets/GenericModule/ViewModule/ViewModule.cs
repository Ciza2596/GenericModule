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
        private List<string> _isVisibleViewNames = new List<string>();
        private List<string> _canUpdateViewNames = new List<string>();

        private List<string> _waitingReleaseViewNames = new List<string>();
        private List<string> _isShowingViewNames = new List<string>();
        private List<string> _isHidingViewNames = new List<string>();

        private Dictionary<string, Action> _onCompletedActions = new Dictionary<string, Action>();


        //zenject callback
        public void Tick()
        {
            var deltaTime = _timeProvider.GetDeltaTime();

            UpdateWaitingReleaseViews(_waitingReleaseViewNames.ToArray());
            UpdateIsShowingViews(_isShowingViewNames.ToArray());
            UpdateIsHidingViews(_isHidingViewNames.ToArray());
            UpdateIsVisibleViews(deltaTime, _isVisibleViewNames.ToArray());
            UpdateViews(deltaTime, _canUpdateViewNames.ToArray());
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

        public bool GetIsVisibleView(string viewName) => _isVisibleViewNames.Contains(viewName);
        public bool GetIsShowing(string viewName) => _views[viewName].IsShowing;
        public bool GetIsHiding(string viewName) => _views[viewName].IsHiding;


        public void LoadView(string viewName, params object[] items)
        {
            Assert.IsTrue(_viewTemplates.ContainsKey(viewName),
                          $"[ViewModule::LoadView] ViewTemplates hasn't viewTemplate: {viewName}.");
            Assert.IsTrue(!_views.ContainsKey(viewName), $"[ViewModule::LoadView] View exists. ViewName: {viewName}.");
            
            
            var viewTemplate = _viewTemplates[viewName];
            var viewGameObject = Object.Instantiate(viewTemplate.GameObject, _viewParentTransform);
            var view = viewGameObject.GetComponent<IView>();
            view.Init(items);

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

            if (_isVisibleViewNames.Contains(viewName))
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

        public void ShowView(string viewName, params object[] items)
        {
            if (_isShowingViewNames.Contains(viewName) || _isVisibleViewNames.Contains(viewName) ||
                _isHidingViewNames.Contains(viewName))
            {
                Debug.LogWarning($"[ViewModule::ShowView] View: {viewName} show fail.");
                return;
            }

            _canUpdateViewNames.Add(viewName);
            _isShowingViewNames.Add(viewName);

            var view = _views[viewName];
            view.Show(items);
        }

        public void HideView(string viewName, Action onCompletedAction = null)
        {
            if (_isHidingViewNames.Contains(viewName) || !_isVisibleViewNames.Contains(viewName) ||
                _isShowingViewNames.Contains(viewName))
            {
                Debug.LogWarning($"[ViewModule::HideView] View: {viewName} hide fail.");
                return;
            }

            if (onCompletedAction != null)
                _onCompletedActions.Add(viewName, onCompletedAction);
            

            _isHidingViewNames.Add(viewName);

            var view = _views[viewName];
            view.Hide();
        }


        //private method
        private void UpdateWaitingReleaseViews(string[] waitingReleaseViewNames)
        {
            foreach (var waitingReleaseViewName in waitingReleaseViewNames)
            {
                if (_isHidingViewNames.Contains(waitingReleaseViewName))
                    continue;

                _waitingReleaseViewNames.Remove(waitingReleaseViewName);

                var view = _views[waitingReleaseViewName];
                view.Release();
                Object.DestroyImmediate(view.GameObject);
            }
        }

        private void UpdateIsShowingViews(string[] isShowingViewNames)
        {
            foreach (var isShowingViewName in isShowingViewNames)
            {
                var view = _views[isShowingViewName];

                if (view.IsShowing)
                    continue;

                _isShowingViewNames.Remove(isShowingViewName);

                _isVisibleViewNames.Add(isShowingViewName);
            }
        }

        private void UpdateIsHidingViews(string[] isHidingViewNames)
        {
            foreach (var isHidingViewName in isHidingViewNames)
            {
                var view = _views[isHidingViewName];

                if (view.IsHiding)
                    continue;

                view.HideAfter();
                _isHidingViewNames.Remove(isHidingViewName);
                _isVisibleViewNames.Remove(isHidingViewName);
                _canUpdateViewNames.Remove(isHidingViewName);

                if (_onCompletedActions.ContainsKey(isHidingViewName))
                {
                    var onCompletedAction = _onCompletedActions[isHidingViewName];
                    _onCompletedActions.Remove(isHidingViewName);
                    onCompletedAction();
                }
            }
        }

        private void UpdateIsVisibleViews(float deltaTime, string[] isVisibleViewNames)
        {
            foreach (var visibleViewNames in isVisibleViewNames)
            {
                if (!_views.ContainsKey(visibleViewNames))
                    continue;

                var view = _views[visibleViewNames];
                view.OnVisibleUpdate(deltaTime);
            }
        }

        private void UpdateViews(float deltaTime, string[] canUpdateViewNames)
        {
            foreach (var canUpdateViewName in canUpdateViewNames)
            {
                if (!_views.ContainsKey(canUpdateViewName))
                    continue;

                var view = _views[canUpdateViewName];
                view.OnUpdate(deltaTime);
            }
        }
    }
}