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

        [Space]
        [SerializeField]
        private GameObject _playerInputManagerPrefab;

        [Space]
        [SerializeField]
        private float _joinedWaitingTime = 0.25f;

        [Space]
        [SerializeField]
        private string _defaultActionMapDataId = "None";


        public string RootName => _rootName;
        public bool IsDontDestroyOnLoad => _isDontDestroyOnLoad;

        public GameObject PlayerInputManagerPrefab => _playerInputManagerPrefab;

        public float JoinedWaitingTime => _joinedWaitingTime;

        public string DefaultActionMapDataId => _defaultActionMapDataId;
    }
}