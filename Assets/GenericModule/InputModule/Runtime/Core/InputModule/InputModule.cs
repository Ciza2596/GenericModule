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


        public bool TryGetPlayerInput(int playerIndex, out PlayerInput playerInput)
        {
            playerInput = _playerInputs.FirstOrDefault(m_playerInput => m_playerInput.playerIndex == playerIndex);
            return playerInput != null;
        }

        public string[] AllControlSchemes
        {
            get
            {
                var controlSchemes = GetPlayerInputPrefab().actions.controlSchemes;
                var allControlSchemes = new HashSet<string>();
                foreach (var controlScheme in controlSchemes)
                    allControlSchemes.Add(controlScheme.name);

                return allControlSchemes.ToArray();
            }
        }


        public bool TryGetCurrentControlScheme(int playerIndex, out string currentControlScheme)
        {
            var playerInput = _playerInputs.FirstOrDefault(m_playerInput => m_playerInput.playerIndex == playerIndex);
            if (playerInput == null)
            {
                currentControlScheme = string.Empty;
                return false;
            }

            currentControlScheme = playerInput.currentControlScheme;
            return true;
        }

        public InputActionAsset GetAndCloneDefaultAsset() =>
            InputActionAsset.FromJson(GetPlayerInputPrefab().actions.ToJson());

        public string GetDefaultActionsJson() =>
            GetAndCloneDefaultAsset().ToJson();

        public bool TryGetActionsJson(int playerIndex, out string json)
        {
            if (!TryGetPlayerInput(playerIndex, out var playerInput))
            {
                json = string.Empty;
                return false;
            }

            json = playerInput.actions.SaveBindingOverridesAsJson();
            return true;
        }

        public bool GetIsInitialized(int playerIndex) =>
            !_timerIdMapByIndex.ContainsKey(playerIndex);

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

        public void RebindActionsByJson(int playerIndex, string json)
        {
            if (string.IsNullOrEmpty(json) || !TryGetPlayerInput(playerIndex, out var playerInput))
                return;

            playerInput.actions.LoadBindingOverridesFromJson(json);
        }

        public void ResetDefaultActions(int playerIndex)
        {
            if (!TryGetPlayerInput(playerIndex, out var playerInput))
                return;

            playerInput.actions.RemoveAllBindingOverrides();
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

            DisableAllInput();
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
            DisableAllInput();

            DestroyPlayerInput();
            DestroyPlayerInputManager();
        }

        public void EnableAllInput()
        {
            IsEnableInput = true;

            foreach (var playerInput in _playerInputs.ToArray())
            {
                if (_timerIdMapByIndex.ContainsKey(playerInput.playerIndex))
                    continue;

                playerInput.ActivateInput();
            }
        }

        public void DisableAllInput()
        {
            IsEnableInput = false;

            foreach (var playerInput in _playerInputs.ToArray())
            {
                if (_timerIdMapByIndex.ContainsKey(playerInput.playerIndex))
                    continue;

                playerInput.DeactivateInput();
            }
        }

        public void EnableInput(int playerIndex)
        {
            if (!TryGetPlayerInput(playerIndex, out var playerInput))
                return;

            playerInput.ActivateInput();
        }

        public void DisableInput(int playerIndex)
        {
            if (!TryGetPlayerInput(playerIndex, out var playerInput))
                return;

            playerInput.DeactivateInput();
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

        public void HandleActionEvent(int playerIndex, Action<int, InputActionAsset> handleEvent)
        {
            if (!TryGetPlayerInput(playerIndex, out var playerInput))
                return;

            handleEvent?.Invoke(playerInput.playerIndex, playerInput.actions);
        }

        public void ResetHaptics(int playerIndex)
        {
            if (!TryGetPlayerInput(playerIndex, out var playerInput))
                return;

            if (!playerInput.TryGetDevices<Gamepad>(out var gamepads))
                return;

            foreach (var gamepad in gamepads)
                gamepad.ResetHaptics();
        }

        public void SetMotorSpeeds(int playerIndex, float lowFrequency, float highFrequency)
        {
            if (!TryGetPlayerInput(playerIndex, out var playerInput))
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

            foreach (var playerInput in _playerInputs.ToArray())
                OnPlayerLeftImp(playerInput);

            var playerInputManager = _playerInputManager;
            _playerInputManager = null;
            Object.Destroy(playerInputManager.gameObject);
        }

        private void CreatePlayerInput()
        {
            if (_playerInput != null)
                DestroyPlayerInput();

            _playerInput = Object.Instantiate(GetPlayerInputPrefab().gameObject, _root).GetComponent<PlayerInput>();
            _playerInput.actions = GetAndCloneDefaultAsset();

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

        private void SwitchActionMap(int playerIndex)
        {
            if (!TryGetPlayerInput(playerIndex, out var playerInput))
                return;

            playerInput.SwitchCurrentActionMap(CurrentActionMapDataId);
        }

        private void SwitchAllActionMap()
        {
            foreach (var playerInput in _playerInputs)
                SwitchActionMap(playerInput.playerIndex);
        }

        private void OnPlayerJoinedImp(PlayerInput playerInput)
        {
            if (_playerInputManager.playerCount >= MaxPlayerCount)
                _playerInputManager.DisableJoining();

            // cant change playerInput.actions. PlayerInputManager will destroy playerInput.
            // playerInput.actions = GetAndCloneDefaultAsset();

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

        private PlayerInput GetPlayerInputPrefab() =>
            _inputModuleConfig.PlayerInputManagerPrefab.GetComponent<PlayerInputManager>().playerPrefab.GetComponent<PlayerInput>();
    }
}