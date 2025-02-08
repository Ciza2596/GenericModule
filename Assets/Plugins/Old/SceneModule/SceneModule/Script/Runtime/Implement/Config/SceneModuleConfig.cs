using UnityEngine;

namespace CizaSceneModule.Implement
{
    [CreateAssetMenu(fileName = "SceneModuleConfig", menuName = "Ciza/SceneModule/SceneModuleConfig")]
    public class SceneModuleConfig : ScriptableObject, ISceneModuleConfig
    {
        [SerializeField]
        private string _transitionSceneName = "TransitionScene";

        public string TransitionSceneName => _transitionSceneName;
    }
}
