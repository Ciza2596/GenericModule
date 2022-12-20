
using System.Collections.Generic;

namespace SceneModule
{
    public interface ITransitionControllerConfig
    {
        public ITransitionView GetTransitionInView(string viewName);

        public ILoadingView GetLoadingView(string viewName);

        public ITransitionView GetTransitionOutView(string viewName);

    }
}
