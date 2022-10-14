using GameCore.Exposed;
using UnityEngine;

namespace ViewModule.Test
{
    public class ExampleView : MonoBehaviour, IView
    {
        //private variable
        private Entry Entry => Entry.Instance;
        private ViewModule ViewModule => Entry.GetModule<ViewModule>();


        //public variable
        public string Name { get; private set; }

        public GameObject GameObject => gameObject;
        public bool IsShowing { get; private set; }

        public bool IsVisible { get; private set; }

        public bool IsHiding { get; private set; }


        //viewModule callback
        public void Init() { }

        public void Show(bool immediately) { }

        public void Hide(bool immediately) { }
        public void OnUpdate(float delta) { }


        //protected method
        protected virtual void StartShow() { }

        protected virtual void StartHide() { }

        //forTest
        public void SetViewName(string viewName)
        {
            Name = viewName;
        }


        public bool SetIsShowing(bool isShowing)
        {
            return IsShowing = isShowing;
        }


        public bool SetIsVisible(bool isVisible)
        {
            return IsVisible = isVisible;
        }


        public bool SetIsHiding(bool isHiding)
        {
            return IsHiding = isHiding;
        }
    }
}