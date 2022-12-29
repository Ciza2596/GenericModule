using UnityEngine;

namespace SceneModule.Example1
{
    [CreateAssetMenu(fileName = "SceneModuleConfig", menuName = "SceneModule/SceneModuleConfig")]
    public class SceneModuleConfig : ScriptableObject, ISceneModuleConfig
    {
        [SerializeField]
        private string _transitionSceneName = "TransitionScene";

        public string TransitionSceneName => _transitionSceneName;
    }
}
