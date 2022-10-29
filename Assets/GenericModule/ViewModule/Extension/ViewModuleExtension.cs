using System;

namespace ViewModule
{
    public static class ViewModuleExtension
    {
        public static void ShowViewThenFadeOut(this ViewModule viewModule, string fadeOutViewName, string showViewName,
            params object[] items)
        {
            viewModule.ShowView(fadeOutViewName, fadeOutViewName);
            viewModule.ShowView(showViewName, items);
        }

        public static void HideViewThenFadeIn(this ViewModule viewModule, string fadeInViewName, string hideViewName,
            Action onCompletedAction = null)
        {
            viewModule.ShowView(fadeInViewName, fadeInViewName, hideViewName, onCompletedAction);
        }

        public static void HideViewThenFadeShow(this ViewModule viewModule, string fadeInViewName, string hideViewName,
            string loadingViewName, string fadeOutViewName, string showViewName)
        {
            viewModule.HideViewThenFadeIn(fadeInViewName, hideViewName,
                () =>
                {
                    viewModule.ShowView(
                        loadingViewName, loadingViewName, fadeOutViewName,
                        showViewName);
                });
        }
    }
}