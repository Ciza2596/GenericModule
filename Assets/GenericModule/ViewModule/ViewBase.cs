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
            OnInit(items);
        }

        public void Show(params object[] items)
        {
            IsHiding = false;
            IsShowing = true;
            ShowCanvasGroup();
            OnShow(items);
        }

        public void Hide()
        {
            IsShowing = false;
            IsHiding = true;
            OnHide();
        }

        public void HideAfter()
        {
            HideCanvasGroup();
        }

        public void Release() => OnRelease();
        public void OnVisibleUpdate(float deltaTime) => OnVisibleUpdateStart(deltaTime);

        public void OnUpdate(float deltaTime)
        {
            OnUpdateStart(deltaTime);
        }


        //protected method
        protected abstract void OnInit(params object[] items);
        protected virtual void OnShow(params object[] items) => IsShowing = false;
        protected virtual void OnHide() => IsHiding = false;
        protected abstract void OnRelease();


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