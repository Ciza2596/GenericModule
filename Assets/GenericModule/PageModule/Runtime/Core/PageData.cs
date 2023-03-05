using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PageModule
{
    internal class PageData
    {
        //public variable
        public Component Page { get; }

        public PageState State { get; private set; }

        //constructor
        public PageData(Component page) =>
            Page = page;


        //public method
        public void Initialize()
        {
            var pageGameObject = Page.gameObject;
            pageGameObject.SetActive(false);
            if (Page is IInitializable initializable)
                initializable.Initialize();
        }

        public void Release()
        {
            if (Page is IReleasable releasable)
                releasable.Release();

            Destroy();
        }


        public bool TryGetTickable(out ITickable tickable)
        {
            tickable = Page as ITickable;
            return tickable != null;
        }

        public bool TryGetFixedTickable(out IFixedTickable fixedTickable)
        {
            fixedTickable = Page as IFixedTickable;
            return fixedTickable != null;
        }


        public async UniTask OnShowingStart(params object[] parameters)
        {
            State = PageState.Showing;

            if (Page is IShowingStart showingStart)
                await showingStart.OnShowingStart(parameters);
            
            var pageGameObject = Page.gameObject;
            pageGameObject.SetActive(true);
        }

        public async UniTask PlayShowingAnimation()
        {
            if (Page is IShowingAnimated showingAnimated)
                await showingAnimated.PlayShowingAnimation();
        }

        public void OnShowingComplete()
        {
            if (Page is IShowingComplete showingComplete)
                showingComplete.OnShowingComplete();

            State = PageState.Visible;
        }


        public void OnHidingStart()
        {
            State = PageState.Hiding;
            
            if(Page is IHidingStart hidingStart)
                hidingStart.OnHidingStart();
        }

        public async UniTask PlayHidingAnimation()
        {
            if(Page is IHidingAnimated hidingAnimated)
                await hidingAnimated.PlayHidingAnimation();
        }

        public void OnHidingComplete()
        {
            if (Page is IHidingComplete hidingComplete)
                hidingComplete.OnHidingComplete();

            var pageGameObject = Page.gameObject;
            pageGameObject.SetActive(false);

            State = PageState.Invisible;
        }

        //private method
        private void Destroy()
        {
            var pageGameObject = Page.gameObject;
            Object.DestroyImmediate(pageGameObject);
        }
    }
}