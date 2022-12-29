using Zenject;


namespace SceneModule.Example1
{
    public class ComponentController : IInitializable
    {
        [Inject] private SceneModule _sceneModule;

        [Inject] private ComponentCollection _componentCollection;

        [Inject] private TransitionSceneData _transitionSceneData;


        public void Initialize()
        {
            _componentCollection.GoToSceneButton.onClick.AddListener(() =>
                _sceneModule.ChangeScene(_transitionSceneData.CurrentSceneName,
                    _transitionSceneData.TransitionInViewName, _transitionSceneData.LoadingViewName,
                    _transitionSceneData.NextSceneName, _transitionSceneData.TransitionOutViewName));
        }
    }
}