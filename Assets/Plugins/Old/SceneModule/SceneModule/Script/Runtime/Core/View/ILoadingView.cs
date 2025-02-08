using System;

namespace CizaSceneModule
{
	public interface ILoadingView
	{
		public void Loading(ILoadSceneAsync loadSceneAsync, ILoadingTask loadingTask, Action activeScene, IInitializingTask initializingTask, Action onComplete);
	}
}
