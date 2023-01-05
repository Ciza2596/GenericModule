using Zenject;


namespace SceneModule.Example1
{
    public class ComponentController : IInitializable
    {
        [Inject] private SceneModule _sceneModule;

        [Inject] private ComponentCollectionData _componentCollectionData;

        [Inject] private TransitionSceneData _transitionSceneData;


        public void Initialize()
        {
            _componentCollectionData.GoToSceneButton.onClick.AddListener(() =>
                _sceneModule.ChangeScene(_transitionSceneData.TransitionInViewName, _transitionSceneData.CurrentSceneName, 
                    _transitionSceneData.LoadingViewName, _transitionSceneData.TransitionOutViewName, _transitionSceneData.NextSceneName));
        }
    }
}