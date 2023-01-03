using UnityEngine;

namespace SceneModule.Example1
{
    [CreateAssetMenu(fileName = "SceneModuleConfig", menuName = "SceneModule/Example1/SceneModuleConfig")]
    public class SceneModuleConfig : ScriptableObject, ISceneModuleConfig
    {
        [SerializeField]
        private string _transitionSceneName = "TransitionScene";

        public string TransitionSceneName => _transitionSceneName;
    }
}
