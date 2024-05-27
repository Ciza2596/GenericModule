using UnityEngine;

namespace CizaFadeCreditModule.Implement
{
    [CreateAssetMenu(fileName = "FadeCreditModuleConfig", menuName = "Ciza/FadeCreditModule/FadeCreditModuleConfig")]
    public class FadeCreditModuleConfig : ScriptableObject, IFadeCreditModuleConfig
    {
        [SerializeField]
        private bool _isDontDestroyOnLoad = true;

        [SerializeField]
        private GameObject _controllerPrefab;

        public bool IsDontDestroyOnLoad => _isDontDestroyOnLoad;

        public GameObject ControllerPrefab => _controllerPrefab;
    }
}