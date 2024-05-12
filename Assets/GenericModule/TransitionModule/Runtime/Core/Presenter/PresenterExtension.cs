using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace CizaTransitionModule
{
    public static class PresenterExtension
    {
        public static UniTask InitializeAsync(this IPresenter[] presenters)
        {
            var uniTasks = new List<UniTask>();
            foreach (var presenter in presenters)
                uniTasks.Add(presenter.InitializeAsync());
            return UniTask.WhenAll(uniTasks);
        }

        public static void Complete(this IPresenter[] presenters)
        {
            foreach (var presenter in presenters)
                presenter.Complete();
        }

        public static void Release(this IPresenter[] presenters)
        {
            foreach (var presenter in presenters)
                presenter.Release();
        }
    }
}