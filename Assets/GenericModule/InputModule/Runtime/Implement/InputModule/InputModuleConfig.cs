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
        private GameObject _eventSystemPrefab;

        [SerializeField]
        private bool _canEnableEventSystem = true;

        [SerializeField]
        private bool _isDefaultEnableEventSystem;

        [Space]
        [SerializeField]
        private bool _isAutoHideEventSystem = true;

        [SerializeField]
        private float _autoHideEventSystemTime = 3;

        [Space]
        [SerializeField]
        private GameObject _playerInputManagerPrefab;

        [SerializeField]
        private float _joinedWaitingTime = 0.25f;

        [Space]
        [SerializeField]
        private string _defaultActionMapDataId = "None";

        [SerializeField]
        private string _disableActionMapDataId = "Disable";


        public string RootName => _rootName;
        public bool IsDontDestroyOnLoad => _isDontDestroyOnLoad;

        public GameObject EventSystemPrefab => _eventSystemPrefab;
        public bool CanEnableEventSystem => _canEnableEventSystem;
        public bool IsDefaultEnableEventSystem => _isDefaultEnableEventSystem;


        public bool IsAutoHideEventSystem => _isAutoHideEventSystem;
        public float AutoHideEventSystemTime => _autoHideEventSystemTime;

        public GameObject PlayerInputManagerPrefab => _playerInputManagerPrefab;
        public float JoinedWaitingTime => _joinedWaitingTime;

        public string DefaultActionMapDataId => _defaultActionMapDataId;
        public string DisableActionMapDataId => _disableActionMapDataId;
    }
}