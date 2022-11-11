using System;

namespace ViewModule
{
    public static class ViewModuleExtension
    {
        public static void ShowViewWithTransition(this ViewModule viewModule, string showViewName, string transitionOutViewName,
            params object[] items)
        {
            viewModule.ShowView(transitionOutViewName, transitionOutViewName);
            viewModule.ShowView(showViewName, items);
        }

        public static void HideViewWithTransition(this ViewModule viewModule, string hideViewName, string transitionInViewName,
            Action onCompleteAction = null)
        {
            viewModule.ShowView(transitionInViewName, hideViewName, transitionInViewName, onCompleteAction);
        }

        public static void ChangeView(this ViewModule viewModule, string hideViewName, string transitionInViewName,
            string loadingViewName, string showViewName, string transitionOutViewName)
        {
            viewModule.HideViewWithTransition(hideViewName, transitionInViewName,
                () =>
                {
                    viewModule.ShowView(
                        loadingViewName, loadingViewName, showViewName,
                        transitionOutViewName);
                });
        }
    }
}