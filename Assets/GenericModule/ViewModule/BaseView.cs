using UnityEngine;

namespace ViewModule
{
    public abstract class BaseView : MonoBehaviour, IView
    {
        public GameObject GameObject => gameObject;
        public bool IsShowing { get; protected set; }
        public bool IsHiding { get; protected set; }

        public void Init() =>
            OnInitStart();
        public void Show()
        {
            IsHiding = false;
            IsShowing = true;
            OnShowStart();
        }
        public void Hide()
        {
            IsShowing = false;
            IsHiding = true;
            OnHideStart();
        }
        public void Release() => OnReleaseStart();
        public void OnUpdate(float deltaTime) => OnUpdateStart(deltaTime);


        //protected method
        protected abstract void OnInitStart();
        protected abstract void OnShowStart();
        protected abstract void OnHideStart();
        protected abstract void OnReleaseStart();


        protected abstract void OnUpdateStart(float deltaTime);
    }
}