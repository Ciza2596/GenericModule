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
        public void Initialize(params object[] parameters)
        {
            var pageGameObject = Page.gameObject;
            pageGameObject.SetActive(false);
            if (Page is IInitializable initializable)
                initializable.Initialize(parameters);
        }

        public void Release()
        {
            if (Page is IReleasable releasable)
                releasable.Release();

            Destroy();
        }


        public bool TryGetUpdateable(out IUpdatable updatable)
        {
            updatable = Page as IUpdatable;
            return updatable != null;
        }

        public bool TryGetFixedUpdateable(out IFixedUpdatable fixedUpdatable)
        {
            fixedUpdatable = Page as IFixedUpdatable;
            return fixedUpdatable != null;
        }


        public async UniTask BeforeShowing(params object[] parameters)
        {
            State = PageState.Showing;

            if (Page is IBeforeShowable beforeShowable)
                await beforeShowable.BeforeShowing(parameters);
        }

        public void Show()
        {
            var pageGameObject = Page.gameObject;
            pageGameObject.SetActive(true);
            
            if (Page is IShowable showable)
                showable.Show();
        }

        public async UniTask ShowingAction()
        {
            if (Page is IShowActionable showActionable)
                await showActionable.ShowingAction();
        }

        public void CompleteShowing()
        {
            if (Page is ICompleteShowable completeShowable)
                completeShowable.CompleteShowing();

            State = PageState.Visible;
        }


        public void Hide()
        {
            State = PageState.Hiding;
            
            if(Page is IHidable hidable)
                hidable.Hide();
        }

        public async UniTask HidingAction()
        {
            if(Page is IHidingActionable hidingActionable)
                await hidingActionable.HidingAction();
        }

        public void CompleteHiding()
        {
            if (Page is ICompleteHidable completeHidable)
                completeHidable.CompleteHiding();

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