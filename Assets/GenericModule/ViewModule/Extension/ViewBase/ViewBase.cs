using UnityEngine;
using UnityEngine.Assertions;

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
        public void Init(params object[] parameters)
        {
            var hasCanvasGroup = TryGetComponent<CanvasGroup>(out var canvasGroup);
            Assert.IsTrue(hasCanvasGroup, $"[ViewBase::Init] View: {GetType()} hasnt canvasGroup.");
            
            _canvasGroup = canvasGroup;

            HideCanvasGroup();
            OnInit(parameters);
        }

        public void Show(params object[] parameters)
        {
            IsHiding = false;
            IsShowing = true;
            ShowCanvasGroup();
            OnShow(parameters);
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
        public void VisibleTick(float deltaTime) => OnVisibleTick(deltaTime);

        public void Tick(float deltaTime)
        {
            OnTick(deltaTime);
        }


        //protected method
        protected abstract void OnInit(params object[] parameters);
        protected virtual void OnShow(params object[] parameters) => IsShowing = false;
        protected virtual void OnHide() => IsHiding = false;
        protected abstract void OnRelease();


        protected abstract void OnVisibleTick(float deltaTime);
        protected virtual void OnTick(float deltaTime) { }


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