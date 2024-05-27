using UnityEngine;

namespace CizaFadeCreditModule.Implement
{
    [CreateAssetMenu(fileName = "FadeCreditModuleConfig", menuName = "Ciza/FadeCreditModule/FadeCreditModuleConfig")]
    public class FadeCreditModuleConfig : ScriptableObject, IFadeCreditModuleConfig
    {
        [SerializeField]
        private string _rootName = "[CreditModule]";

        [Space]
        [SerializeField]
        private bool _isDontDestroyOnLoad = true;

        [SerializeField]
        private GameObject _controllerPrefab;

        public string RootName => _rootName;

        public bool IsDontDestroyOnLoad => _isDontDestroyOnLoad;

        public GameObject ControllerPrefab => _controllerPrefab;
    }
}