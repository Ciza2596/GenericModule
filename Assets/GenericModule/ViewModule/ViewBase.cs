using UnityEngine;

namespace ViewModule
{
    public abstract class ViewBase : MonoBehaviour, IView
    {
        //private variable
        private CanvasGroup _canvasGroup;


        //public variable
        public GameObject GameObject => gameObject;
        public bool IsShowing { get; protected set; }
        public bool IsHiding { get; protected set; }


        //view callback
        public void Init(params object[] items)
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            HideCanvasGroup();
            OnInitStart(items);
        }

        public void Show(params object[] items)
        {
            IsHiding = false;
            IsShowing = true;
            ShowCanvasGroup();
            OnShowStart(items);
        }

        public void Hide()
        {
            IsShowing = false;
            IsHiding = true;
            OnHideStart();
        }

        public void HideAfter()
        {
            HideCanvasGroup();
        }

        public void Release() => OnReleaseStart();
        public void OnVisibleUpdate(float deltaTime) => OnVisibleUpdateStart(deltaTime);

        public void OnUpdate(float deltaTime)
        {
            OnUpdateStart(deltaTime);
        }


        //protected method
        protected abstract void OnInitStart(params object[] items);
        protected virtual void OnShowStart(params object[] items) => IsShowing = false;
        protected virtual void OnHideStart() => IsHiding = false;
        protected abstract void OnReleaseStart();


        protected abstract void OnVisibleUpdateStart(float deltaTime);
        protected abstract void OnUpdateStart(float deltaTime);
        
        
        //private method
        private void ShowCanvasGroup()
        {
            _canvasGroup.alpha = 1;
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
        }

        private void HideCanvasGroup()
        {
            _canvasGroup.alpha = 0;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }
    }
}