using System.Collections.Generic;
using System.Threading.Tasks;
using GameCore.Exposed;
using UnityEngine;


namespace ViewModule
{
    public class ViewModule : BaseModule
    {
        //private variable
        private LogModule _logModule;
        private ResourcesModule _resourcesModule;

        private const string VIEW_PARENT_TRASFORM_NAME = "ViewParent";

        private List<GameObject> _viewPrefabs;

        private Dictionary<string, IView> _views;
        private List<IView> _visibleViews;

        private Transform _viewParentTransform;

        //public variable
        public const string VIEW_DATA_OVERVIEW = "ViewDataOverView";
        public IReadOnlyDictionary<string, IView> Views => _views;
        public IList<IView> VisibleViews => _visibleViews;


        //module flow
        public override void Initialize(params object[] param)
        {
            if (param.Length != 2 || !(param[0] is LogModule logModule) ||
                !(param[1] is ViewDataOverview viewDataOverview))
            {
                _logModule.Log(LogModule.LogLevels.Error, "[GameLoopModule::Initialize] fail.");
                return;
            }

            _logModule = logModule;
            _viewPrefabs = viewDataOverview.GetViewPrefabs();

            _views = new Dictionary<string, IView>();
            _visibleViews = new List<IView>();

            _viewParentTransform = new GameObject(VIEW_PARENT_TRASFORM_NAME).transform;

        }

        public override void Release(params object[] param)
        {
            _viewPrefabs = null;
            _views = null;
            _visibleViews = null;

            Object.DestroyImmediate(_viewParentTransform.gameObject);
        }

        public override void OnUpdate(float delta)
        {
            foreach (var visibleView in VisibleViews)
                visibleView.OnUpdate(delta);
        }


        //public method
        public void RegisterView(IView view)
        {
            var viewName = view.Name;

            if (_views.ContainsKey(viewName))
            {
                _logModule.Log(LogModule.LogLevels.Debug,
                               $"[ViewModule::RegisterView] View is registered. ViewName: {viewName}");
                return;
            }

            _views.Add(viewName, view);
        }

        public void LoadView(string viewName)
        {
            if (_views.ContainsKey(viewName))
            {
                _logModule.Log(LogModule.LogLevels.Debug, $"[ViewModule::LoadView] View exists. ViewName: {viewName}");
                return;
            }

            var viewPrefab = _viewPrefabs.Find(prefab => prefab.GetComponent<IView>().Name == viewName);
            var viewGameObject = Object.Instantiate(viewPrefab, _viewParentTransform);
            var view = viewGameObject.GetComponent<IView>();
            view.Init();

            _views.Add(viewName, view);
        }

        public void LoadAllView()
        {
            foreach (var viewPrefab in _viewPrefabs)
            {
                var viewName = viewPrefab.GetComponent<IView>().Name;
                LoadView(viewName);
            }
        }

        public void ReleaseView(string viewName)
        {
            if (!_views.ContainsKey(viewName))
            {
                _logModule.Log(LogModule.LogLevels.Debug,
                               $"[ViewModule::ReleaseView] View not exists. ViewName: {viewName}");
                return;
            }

            var view = _views[viewName];
            _views.Remove(viewName);

            var viewGameObject = view.GameObject;
            Object.DestroyImmediate(viewGameObject);
        }

        public void ReleaseAllView()
        {
            foreach (var view in _views)
            {
                var viewName = view.Value.Name;
                LoadView(viewName);
            }
        }


        public bool GetHasView(string viewName)
        {
            return _views.ContainsKey(viewName);
        }

        public IView GetView(string viewName)
        {
            if (_views.TryGetValue(viewName, out var page))
            {
                return page;
            }

            _logModule.Log(LogModule.LogLevels.Warn, $"[ViewModule::GetView] Can't find view with name : {viewName}");
            return null;
        }

        public void ShowView(string viewName, bool isImmediately = false)
        {
            var view = GetView(viewName);
            if (view == null)
            {
                _logModule.Log(LogModule.LogLevels.Error,
                               $"[ViewModule::ShowView] View not exists. ViewName: {viewName}");
                return;
            }

            view.Show(isImmediately);
            _visibleViews.Add(view);
        }

        public async Task ShowViewAsync(string viewName)
        {
            var view = GetView(viewName);
            if (view == null)
            {
                _logModule.Log(LogModule.LogLevels.Error,
                               $"[ViewModule::ShowViewAsync] View not exists. ViewName: {viewName}");
                return;
            }


            view.Show(false);
            while (view.IsShowing) await Task.Yield();
            _visibleViews.Add(view);
        }


        public void HideView(string viewName, bool isImmediately = false)
        {
            var view = GetView(viewName);
            if (view == null)
            {
                _logModule.Log(LogModule.LogLevels.Error,
                               $"[ViewModule::HideView] View not exists. ViewName: {viewName}");
                return;
            }

            view.Hide(isImmediately);
            _visibleViews.Remove(view);
        }

        public async Task HideViewAsync(string viewName)
        {
            var view = GetView(viewName);
            if (view == null)
            {
                _logModule.Log(LogModule.LogLevels.Error,
                               $"[ViewModule::HideViewAsync] View not exists. ViewName: {viewName}");
                return;
            }

            view.Hide(false);
            while (view.IsHiding) await Task.Yield();
            _visibleViews.Remove(view);
        }


        public void HideAllView(bool immediately = true)
        {
            foreach (var viewPair in _views)
            {
                var view = viewPair.Value;

                view.Hide(immediately);
                _visibleViews.Remove(view);
            }
        }
    }
}