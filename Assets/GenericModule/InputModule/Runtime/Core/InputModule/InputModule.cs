using System;
using System.Collections.Generic;
using System.Linq;
using CizaTimerModule;
using UnityEngine;
using UnityEngine.InputSystem;
using Object = UnityEngine.Object;

namespace CizaInputModule
{
    public class InputModule
    {
        private readonly IInputModuleConfig _inputModuleConfig;

        private readonly HashSet<PlayerInput> _playerInputs = new HashSet<PlayerInput>();
        private readonly Dictionary<int, string> _timerIdMapByIndex = new Dictionary<int, string>();

        private readonly TimerModule _timerModule = new TimerModule();

        private Transform _root;

        private GameObject _eventSystem;

        private PlayerInputManager _playerInputManager;
        private PlayerInput _playerInput;

        private string _currentActionMapDataId;
        private string _previousControlScheme;

        public event Action<PlayerInput> OnControlsChanged;

        public event Action<PlayerInput> OnPlayerJoined;
        public event Action<PlayerInput> OnPlayerLeft;

        public bool IsInitialized { get; private set; }

        public bool CanEnableEventSystem { get; private set; }
        public bool IsEnableEventSystem => _eventSystem != null ? _eventSystem.activeSelf : false;
        public bool IsEnableJoining { get; private set; }

        public bool IsEnableInput { get; private set; }

        public bool IsSinglePlayer { get; private set; }

        public string CurrentActionMapDataId => !string.IsNullOrEmpty(_currentActionMapDataId) ? _currentActionMapDataId : _inputModuleConfig.DefaultActionMapDataId;

        public int PlayerCount
        {
            get
            {
                if (IsSinglePlayer)
                    return _playerInput != null ? 1 : 0;

                return _playerInputManager != null ? _playerInputManager.playerCount : 0;
            }
        }

        public int MaxPlayerCount { get; private set; }

        public bool TryGetPlayerInput(int index, out PlayerInput playerInput)
        {
            playerInput = _playerInputs.FirstOrDefault(m_playerInput => m_playerInput.playerIndex == index);
            return playerInput != null;
        }

        public bool TryGetCurrentControlScheme(int index, out string currentControlScheme)
        {
            var playerInput = _playerInputs.FirstOrDefault(m_playerInput => m_playerInput.playerIndex == index);
            if (playerInput == null)
            {
                currentControlScheme = string.Empty;
                return false;
            }

            currentControlScheme = playerInput.currentControlScheme;
            return true;
        }

        public bool GetIsInitialized(int index) =>
            !_timerIdMapByIndex.ContainsKey(index);

        public InputModule(IInputModuleConfig inputModuleConfig) =>
            _inputModuleConfig = inputModuleConfig;

        public void Initialize(Transform parent = null)
        {
            if (IsInitialized)
                return;

            IsInitialized = true;

            var rootGameObject = new GameObject(_inputModuleConfig.RootName);
            _root = rootGameObject.transform;

            if (parent != null)
                _root.SetParent(parent);

            else if (_inputModuleConfig.IsDontDestroyOnLoad)
                Object.DontDestroyOnLoad(_root.gameObject);

            _eventSystem = Object.Instantiate(_inputModuleConfig.EventSystemPrefab, _root);
            SetCanEnableEventSystem(_inputModuleConfig.CanEnableEventSystem);

            if (_inputModuleConfig.IsDefaultEnableEventSystem)
                EnableEventSystem();
            else
                DisableEventSystem();

            _timerModule.Initialize();
        }

        public void Release()
        {
            if (!IsInitialized)
                return;

            Clear();
            _timerModule.Release();

            var eventSystem = _eventSystem;
            _eventSystem = null;
            Object.Destroy(eventSystem);

            var root = _root;
            _root = null;
            Object.Destroy(root.gameObject);

            IsInitialized = false;
        }

        public void Tick(float deltaTime) =>
            _timerModule.Tick(deltaTime);

        public void SetCanEnableEventSystem(bool canEnableEventSystem)
        {
            CanEnableEventSystem = canEnableEventSystem;
            if (!CanEnableEventSystem && IsEnableEventSystem)
                DisableEventSystem();
        }

        public void EnableEventSystem()
        {
            if (!IsInitialized || !CanEnableEventSystem || IsEnableEventSystem)
                return;

            _eventSystem.SetActive(true);
        }

        public void DisableEventSystem()
        {
            if (!IsInitialized || !IsEnableEventSystem)
                return;

            _eventSystem.SetActive(false);
        }

        public void SetMaxPlayerCount(int maxPlayerCount) =>
            MaxPlayerCount = maxPlayerCount;

        public void StartJoining(int maxPlayerCount)
        {
            SetMaxPlayerCount(maxPlayerCount);
            StartJoining();
        }

        public void StartJoining()
        {
            if (!IsInitialized)
                return;

            Clear();

            if (MaxPlayerCount > 1)
            {
                IsSinglePlayer = false;
                CreatePlayerInputManager();
            }
            else
            {
                IsSinglePlayer = true;
                CreatePlayerInput();
            }

            DisableInput();
        }

        public void EnableJoining()
        {
            if (!IsInitialized)
                return;

            IsEnableJoining = true;

            if (_playerInputManager != null)
                _playerInputManager.EnableJoining();
        }

        public void DisableJoining()
        {
            if (!IsInitialized)
                return;

            IsEnableJoining = false;

            if (_playerInputManager != null)
                _playerInputManager.DisableJoining();
        }

        public void Clear()
        {
            if (!IsInitialized)
                return;

            DisableJoining();
            DisableInput();

            DestroyPlayerInput();
            DestroyPlayerInputManager();
        }

        public void EnableInput()
        {
            IsEnableInput = true;

            foreach (var playerInput in _playerInputs.ToArray())
            {
                if (_timerIdMapByIndex.ContainsKey(playerInput.playerIndex))
                    continue;

                playerInput.actions.Enable();
            }
        }

        public void DisableInput()
        {
            IsEnableInput = false;

            foreach (var playerInput in _playerInputs.ToArray())
            {
                if (_timerIdMapByIndex.ContainsKey(playerInput.playerIndex))
                    continue;

                playerInput.actions.Disable();
            }
        }

        public void SetCurrentActionMapDataId(string actionMapDataId)
        {
            _currentActionMapDataId = actionMapDataId;
            SwitchAllActionMap();
        }

        public void HandleActionEvent(Action<int, InputActionAsset> handleEvent)
        {
            foreach (var playerInput in _playerInputs)
                HandleActionEvent(playerInput.playerIndex, handleEvent);
        }

        public void HandleActionEvent(int index, Action<int, InputActionAsset> handleEvent)
        {
            if (!TryGetPlayerInput(index, out var playerInput))
                return;

            handleEvent?.Invoke(playerInput.playerIndex, playerInput.actions);
        }

        public void ResetHaptics(int index)
        {
            if (!TryGetPlayerInput(index, out var playerInput))
                return;

            if (!playerInput.TryGetDevices<Gamepad>(out var gamepads))
                return;

            foreach (var gamepad in gamepads)
                gamepad.ResetHaptics();
        }

        public void SetMotorSpeeds(int index, float lowFrequency, float highFrequency)
        {
            if (!TryGetPlayerInput(index, out var playerInput))
                return;

            if (!playerInput.TryGetDevices<Gamepad>(out var gamepads))
                return;

            foreach (var gamepad in gamepads)
                gamepad.SetMotorSpeeds(lowFrequency, highFrequency);
        }

        private void CreatePlayerInputManager()
        {
            if (_playerInputManager != null)
                DestroyPlayerInputManager();

            _playerInputManager = Object.Instantiate(_inputModuleConfig.PlayerInputManagerPrefab, _root).GetComponent<PlayerInputManager>();
            DisableJoining();
            _playerInputManager.onPlayerJoined += OnPlayerJoinedImp;
            _playerInputManager.onPlayerLeft += OnPlayerLeftImp;
            EnableJoining();
        }

        private void DestroyPlayerInputManager()
        {
            if (_playerInputManager is null)
                return;

            var playerInputManager = _playerInputManager;

            foreach (var playerInput in _playerInputs.ToArray())
                OnPlayerLeftImp(playerInput);

            _playerInputManager = null;
            Object.Destroy(playerInputManager.gameObject);
        }

        private void CreatePlayerInput()
        {
            if (_playerInput != null)
                DestroyPlayerInput();

            _playerInput = Object.Instantiate(_inputModuleConfig.PlayerInputManagerPrefab.GetComponent<PlayerInputManager>().playerPrefab, _root).GetComponent<PlayerInput>();

            if (IsEnableInput)
                _playerInput.actions.Enable();
            else
                _playerInput.actions.Disable();

            _playerInputs.Add(_playerInput);
            OnPlayerJoined?.Invoke(_playerInput);

            if (TryGetPreviousControlScheme(out var previousControlScheme))
                _playerInput.SwitchCurrentControlScheme(previousControlScheme);

            OnControlsChangedImp(_playerInput);
            _playerInput.onControlsChanged += OnControlsChangedImp;

            SwitchActionMap(_playerInput.playerIndex);
        }

        private void DestroyPlayerInput()
        {
            if (_playerInput is null)
                return;

            var playerInput = _playerInput;
            _playerInput = null;

            _playerInputs.Remove(playerInput);
            OnPlayerLeft?.Invoke(playerInput);

            Object.Destroy(playerInput.gameObject);
        }

        private void SwitchActionMap(int index)
        {
            if (!TryGetPlayerInput(index, out var playerInput))
                return;

            playerInput.SwitchCurrentActionMap(CurrentActionMapDataId);
        }

        private void SwitchAllActionMap()
        {
            for (var i = 0; i < _playerInputs.Count; i++)
                SwitchActionMap(i);
        }

        private void OnPlayerJoinedImp(PlayerInput playerInput)
        {
            if (_playerInputManager.playerCount >= MaxPlayerCount)
                _playerInputManager.DisableJoining();

            playerInput.actions.Disable();
            var timerId = _timerModule.AddOnceTimer(_inputModuleConfig.JoinedWaitingTime, timerReadModel =>
            {
                if (IsEnableInput)
                    playerInput.actions.Enable();

                _timerIdMapByIndex.Remove(playerInput.playerIndex);
            });

            _timerIdMapByIndex.Add(playerInput.playerIndex, timerId);

            playerInput.transform.SetParent(_playerInputManager.transform);
            _playerInputs.Add(playerInput);
            OnPlayerJoined?.Invoke(playerInput);

            SwitchActionMap(playerInput.playerIndex);
        }

        private void OnPlayerLeftImp(PlayerInput playerInput)
        {
            if (IsEnableJoining && _playerInputManager.playerCount < MaxPlayerCount)
                _playerInputManager.EnableJoining();

            if (_timerIdMapByIndex.TryGetValue(playerInput.playerIndex, out var timerId))
            {
                _timerModule.RemoveTimer(timerId);
                _timerIdMapByIndex.Remove(playerInput.playerIndex);
            }

            _playerInputs.Remove(playerInput);
            OnPlayerLeft?.Invoke(playerInput);
        }

        private void OnControlsChangedImp(PlayerInput playerInput)
        {
            OnControlsChanged?.Invoke(playerInput);
            _previousControlScheme = playerInput.currentControlScheme;
        }


        private bool TryGetPreviousControlScheme(out string previousControlScheme)
        {
            if (string.IsNullOrEmpty(_previousControlScheme) || string.IsNullOrWhiteSpace(_previousControlScheme))
            {
                previousControlScheme = string.Empty;
                return false;
            }

            previousControlScheme = _previousControlScheme;
            return true;
        }
    }
}