using UnityEngine;

namespace CizaInputModule.Implement
{
    [CreateAssetMenu(fileName = "InputModuleConfig", menuName = "Ciza/InputModule/InputModuleConfig", order = -1)]
    public class InputModuleConfig : ScriptableObject, IInputModuleConfig
    {
        [SerializeField]
        private string _rootName = "[InputModule]";

        [SerializeField]
        private bool _isDontDestroyOnLoad = true;

        [SerializeField]
        private GameObject _playerInputManagerPrefab;


        public string RootName => _rootName;
        public bool IsDontDestroyOnLoad => _isDontDestroyOnLoad;
        public GameObject PlayerInputManagerPrefab => _playerInputManagerPrefab;
    }
}