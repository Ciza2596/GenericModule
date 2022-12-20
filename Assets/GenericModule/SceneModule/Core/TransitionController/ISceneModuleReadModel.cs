namespace SceneModule
{
    public interface ISceneModuleReadModel
    {
        string TransitionInViewName { get; }
        string CurrentSceneName { get; }
        
        string LoadingViewName { get; }
        string NextSceneName { get; }
        
        
        string TransitionOutViewName { get; }
        string TransitionSceneName { get; }
    }
}