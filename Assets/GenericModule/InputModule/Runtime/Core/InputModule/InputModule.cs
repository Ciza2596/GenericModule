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

        private PlayerInputManager _playerInputManager;
        private PlayerInput _playerInput;

        private string _currentActionMapDataId;

        public event Action<PlayerInput> OnControlsChanged;

        public event Action<PlayerInput> OnPlayerJoined;
        public event Action<PlayerInput> OnPlayerLeft;

        public bool IsInitialized { get; private set; }

        public bool IsEnableJoining { get; private set; }

        public bool IsEnableInput { get; private set; }

        public bool IsSinglePlayer { get; private set; }

        public string CurrentActionMapDataId => !string.IsNullOrEmpty(_currentActionMapDataId) ? _currentActionMapDataId : _inputModuleConfig.DefaultActionMapDataId;

        public int PlayerCount => IsSinglePlayer || _playerInputManager is null ? 1 : _playerInputManager.playerCount;
        public int MaxPlayerCount { get; private set; }

        public bool TryGetPlayerInput(int index, out PlayerInput playerInput)
        {
            playerInput = _playerInputs.FirstOrDefault(m_playerInput => m_playerInput.playerIndex == index);
            return playerInput != null;
        }

        public bool GetIsInitialized(int index) =>
            !_timerIdMapByIndex.ContainsKey(index);

        public InputModule(IInputModuleConfig inputModuleConfig) =>
            _inputModuleConfig = inputModuleConfig;

        public void Initialize(Transform parent)
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

            _timerModule.Initialize();
        }

        public void Release()
        {
            if (!IsInitialized)
                return;

            IsInitialized = false;

            Clear();
            _timerModule.Release();

            var root = _root;
            _root = null;
            Object.Destroy(root.gameObject);
        }

        public void Tick(float deltaTime) =>
            _timerModule.Tick(deltaTime);

        public void SetMaxPlayerCount(int maxPlayerCount) =>
            MaxPlayerCount = maxPlayerCount;

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

        public void HandlePlayerInputsEvent(Action<PlayerInput> handleEvent)
        {
            foreach (var playerInput in _playerInputs)
                HandlePlayerInputEvent(playerInput.playerIndex, handleEvent);
        }

        public void HandlePlayerInputEvent(int index, Action<PlayerInput> handleEvent)
        {
            if (!TryGetPlayerInput(index, out var playerInput))
                return;

            handleEvent?.Invoke(playerInput);
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

        private void OnControlsChangedImp(PlayerInput playerInput) =>
            OnControlsChanged?.Invoke(playerInput);
    }
}