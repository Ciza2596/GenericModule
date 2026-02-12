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
		// VARIABLE: -----------------------------------------------------------------------------

		protected readonly IInputModuleConfig _inputModuleConfig;

		protected readonly HashSet<PlayerInput> _playerInputs = new HashSet<PlayerInput>();
		protected readonly Dictionary<int, string> _timerIdMapByIndex = new Dictionary<int, string>();

		protected readonly TimerModule _timerModule = new TimerModule();
		protected readonly VirtualMouseContainer _virtualMouseContainer;

		protected Transform _root;
		protected BEventHandler[] _eventHandlers;

		protected GameObject _eventSystem;

		protected PlayerInputManager _playerInputManager;
		protected PlayerInput _playerInput;

		protected string _currentActionMapDataId;
		protected string _previousControlScheme;

		protected float _autoHideHardwareCursorTimer;

		// EVENT: ---------------------------------------------------------------------------------

		public event Action<PlayerInput> OnControlsChanged;

		public event Action<PlayerInput> OnPlayerJoined;
		public event Action<PlayerInput> OnPlayerLeft;

		public event Action<PlayerInput, float> OnTick;


		#region GetActionPath

		// PlayerIndex, CurrentActionMapDataId, Returns Path
		public event Func<int, string, string> GetVirtualMouseMoveActionPath;

		// PlayerIndex, CurrentActionMapDataId, Returns Path
		public event Func<int, string, string> GetVirtualMouseLeftButtonActionPath;

		// PlayerIndex, CurrentActionMapDataId, Returns Path
		public event Func<int, string, string> GetVirtualMouseRightButtonActionPath;

		// PlayerIndex, CurrentActionMapDataId, Returns Path
		public event Func<int, string, string> GetVirtualMouseScrollActionPath;

		#endregion

		// PUBLIC VARIABLE: ---------------------------------------------------------------------

		public virtual bool IsInitialized { get; private set; }

		public virtual bool CanEnableEventSystem { get; private set; }
		public virtual bool IsEnableEventSystem { get; private set; }
		public virtual bool IsHardwareCursorVisible { get; private set; }

		public virtual bool IsEnableJoining { get; private set; }

		public virtual bool IsEnableInput { get; private set; }

		public virtual bool IsSinglePlayer { get; private set; }

		public virtual string CurrentActionMapDataId => !string.IsNullOrEmpty(_currentActionMapDataId) ? _currentActionMapDataId : _inputModuleConfig.DefaultActionMapDataId;

		public virtual int PlayerCount
		{
			get
			{
				if (IsSinglePlayer)
					return _playerInput != null ? 1 : 0;

				return _playerInputManager != null ? _playerInputManager.playerCount : 0;
			}
		}

		public virtual int MaxPlayerCount { get; private set; }

		public virtual bool TryGetPlayerInput(out PlayerInput playerInput) =>
			TryGetPlayerInput(0, out playerInput);

		public virtual bool TryGetPlayerInput(int playerIndex, out PlayerInput playerInput)
		{
			playerInput = _playerInputs.FirstOrDefault(m_playerInput => m_playerInput.playerIndex == playerIndex);
			return playerInput != null;
		}

		public virtual string[] AllControlSchemes
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


		public virtual bool TryGetCurrentControlScheme(int playerIndex, out string currentControlScheme)
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

		public virtual InputActionAsset GetAndCloneDefaultAsset() =>
			InputActionAsset.FromJson(GetPlayerInputPrefab().actions.ToJson());

		public virtual string GetDefaultActionsJson() =>
			GetAndCloneDefaultAsset().ToJson();

		public virtual bool TryGetActionsJson(int playerIndex, out string json)
		{
			if (!TryGetPlayerInput(playerIndex, out var playerInput))
			{
				json = string.Empty;
				return false;
			}

			json = playerInput.actions.SaveBindingOverridesAsJson();
			return true;
		}

		public virtual bool GetIsInitialized(int playerIndex) =>
			!_timerIdMapByIndex.ContainsKey(playerIndex);


		#region VirtualMouse

		public virtual bool CanEnableVirtualMouse => _virtualMouseContainer.CanEnableVirtualMouse;

		public virtual bool TryGetVirtualMouseReadModel(int playerIndex, out IVirtualMouseReadModel virtualMouseReadModel) =>
			_virtualMouseContainer.TryGetVirtualMouseReadModel(playerIndex, out virtualMouseReadModel);

		#endregion

		// CONSTRUCTOR: ------------------------------------------------------------------------

		public InputModule(IInputModuleConfig inputModuleConfig)
		{
			_inputModuleConfig = inputModuleConfig;
			_virtualMouseContainer = new VirtualMouseContainer(_inputModuleConfig);
			_virtualMouseContainer.GetMoveActionPath += GetVirtualMouseMoveActionPathImp;
			_virtualMouseContainer.GetLeftButtonActionPath += GetVirtualMouseLeftButtonActionPathImp;
			_virtualMouseContainer.GetRightButtonActionPath += GetVirtualMouseRightButtonActionPathImp;
			_virtualMouseContainer.GetScrollActionPath += GetVirtualMouseScrollActionPathImp;
		}

		// LIFECYCLE METHOD: ------------------------------------------------------------------

		public virtual void Initialize(Transform parent = null) =>
			Initialize(Array.Empty<BEventHandler>(), parent);

		public virtual void Initialize(BEventHandler eventHandler, Transform parent = null) =>
			Initialize(new BEventHandler[] { eventHandler }, parent);

		public virtual void Initialize(BEventHandler[] eventHandlers, Transform parent = null)
		{
			if (IsInitialized)
				return;

			var rootGameObject = new GameObject(_inputModuleConfig.RootName);
			_root = rootGameObject.transform;

			if (parent != null)
				_root.SetParent(parent);

			else if (_inputModuleConfig.IsDontDestroyOnLoad)
				Object.DontDestroyOnLoad(_root.gameObject);

			_eventSystem = Object.Instantiate(_inputModuleConfig.EventSystemPrefab, _root);
			DisableEventSystem();
			SetCanEnableEventSystem(_inputModuleConfig.CanEnableEventSystem);

			_virtualMouseContainer.Initialize(_root);
			_virtualMouseContainer.SetCanEnableVirtualMouse(_inputModuleConfig.CanEnableVirtualMouse);

			if (_inputModuleConfig.IsDefaultEnableEventSystem)
				EnableEventSystem();
			else
				DisableEventSystem();

			_timerModule.Initialize();

			_eventHandlers = eventHandlers;
			foreach (var eventHandler in _eventHandlers)
				eventHandler.Register(this);

			IsInitialized = true;
		}

		public virtual void Release()
		{
			if (!IsInitialized)
				return;

			Clear();
			foreach (var eventHandler in _eventHandlers)
				eventHandler.Unregister(this);
			_eventHandlers = Array.Empty<BEventHandler>();

			_timerModule.Release();
			_virtualMouseContainer.Release();

			var eventSystem = _eventSystem;
			_eventSystem = null;
			Object.Destroy(eventSystem);

			var root = _root;
			_root = null;
			Object.Destroy(root.gameObject);

			IsInitialized = false;
		}

		public virtual void Tick(float deltaTime)
		{
			if (!IsInitialized)
				return;

			CheckAutoHideHardwareCursorTime(deltaTime);
			CheckAutoHideEventSystem();

			_timerModule.Tick(deltaTime);

			foreach (var playerInput in _playerInputs.ToArray())
				if (playerInput.currentActionMap.name != _inputModuleConfig.DisableActionMapDataId)
					OnTick?.Invoke(playerInput, deltaTime);
		}

		// PUBLIC METHOD: ----------------------------------------------------------------------

		#region EventSystem

		public virtual void SetCanEnableEventSystem(bool canEnableEventSystem)
		{
			CanEnableEventSystem = canEnableEventSystem;
			if (!CanEnableEventSystem && IsEnableEventSystem)
				DisableEventSystem();
		}

		public virtual void EnableEventSystem()
		{
			if (!IsInitialized || !CanEnableEventSystem)
				return;

			if (!_eventSystem.activeSelf)
				_eventSystem.SetActive(true);

			IsEnableEventSystem = true;
		}

		public virtual void DisableEventSystem()
		{
			if (!IsInitialized)
				return;

			if (_eventSystem != null && _eventSystem.activeSelf)
				_eventSystem.SetActive(false);

			IsEnableEventSystem = false;
		}

		#endregion

		public virtual void RebindActionsByJson(int playerIndex, string json)
		{
			if (string.IsNullOrEmpty(json) || !TryGetPlayerInput(playerIndex, out var playerInput))
				return;

			playerInput.actions.LoadBindingOverridesFromJson(json);
		}

		public virtual void ResetDefaultActions(int playerIndex)
		{
			if (!TryGetPlayerInput(playerIndex, out var playerInput))
				return;

			playerInput.actions.RemoveAllBindingOverrides();
		}

		public virtual void SetMaxPlayerCount(int maxPlayerCount) =>
			MaxPlayerCount = maxPlayerCount;

		public virtual void StartJoining(int maxPlayerCount)
		{
			SetMaxPlayerCount(maxPlayerCount);
			StartJoining();
		}

		public virtual void StartJoining()
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

		public virtual void EnableJoining()
		{
			if (!IsInitialized)
				return;

			IsEnableJoining = true;

			if (_playerInputManager != null)
				_playerInputManager.EnableJoining();
		}

		public virtual void DisableJoining()
		{
			if (!IsInitialized)
				return;

			IsEnableJoining = false;

			if (_playerInputManager != null)
				_playerInputManager.DisableJoining();
		}

		public virtual void Clear()
		{
			if (!IsInitialized)
				return;

			DisableJoining();
			DisableAllInput();

			DestroyPlayerInput();
			DestroyPlayerInputManager();
		}

		public virtual void EnableAllInput()
		{
			IsEnableInput = true;

			foreach (var playerInput in _playerInputs.ToArray())
				if (!_timerIdMapByIndex.ContainsKey(playerInput.playerIndex))
					EnableInput(playerInput.playerIndex);

			if (!IsEnableEventSystem)
				EnableEventSystem();
		}

		public virtual void DisableAllInput()
		{
			IsEnableInput = false;

			foreach (var playerInput in _playerInputs.ToArray())
				if (!_timerIdMapByIndex.ContainsKey(playerInput.playerIndex))
					DisableInput(playerInput.playerIndex);

			DisableEventSystem();
		}

		public virtual void EnableInput(int playerIndex)
		{
			SwitchCurrentActionMap(playerIndex);
			_virtualMouseContainer.Enable(playerIndex);
		}

		public virtual void DisableInput(int playerIndex)
		{
			SwitchActionMap(playerIndex, _inputModuleConfig.DisableActionMapDataId);
			_virtualMouseContainer.Disable(playerIndex);
		}

		public virtual void SetCurrentActionMapDataId(string actionMapDataId) =>
			_currentActionMapDataId = actionMapDataId;

		public virtual void HandleActionEvent(Action<int, InputActionAsset> handleEvent)
		{
			foreach (var playerInput in _playerInputs)
				HandleActionEvent(playerInput.playerIndex, handleEvent);
		}

		public virtual void HandleActionEvent(int playerIndex, Action<int, InputActionAsset> handleEvent)
		{
			if (!TryGetPlayerInput(playerIndex, out var playerInput))
				return;

			handleEvent?.Invoke(playerInput.playerIndex, playerInput.actions);
		}

		public virtual void ResetHaptics(int playerIndex)
		{
			if (!TryGetPlayerInput(playerIndex, out var playerInput))
				return;

			if (!playerInput.TryGetDevices<Gamepad>(out var gamepads))
				return;

			foreach (var gamepad in gamepads)
				gamepad.ResetHaptics();
		}

		public virtual void SetMotorSpeeds(int playerIndex, float lowFrequency, float highFrequency)
		{
			if (!TryGetPlayerInput(playerIndex, out var playerInput))
				return;

			if (!playerInput.TryGetDevices<Gamepad>(out var gamepads))
				return;

			foreach (var gamepad in gamepads)
				gamepad.SetMotorSpeeds(lowFrequency, highFrequency);
		}


		#region VirtualMouse

		public virtual void SetCanEnableVirtualMouse(bool canEnableVirtualMouse) =>
			_virtualMouseContainer.SetCanEnableVirtualMouse(canEnableVirtualMouse);


		public virtual void EnableVirtualMouse(int playerIndex) =>
			_virtualMouseContainer.Enable(playerIndex);

		public virtual void EnableAllVirtualMouse() =>
			_virtualMouseContainer.EnableAll();


		public virtual void DisableVirtualMouse(int playerIndex) =>
			_virtualMouseContainer.Disable(playerIndex);

		public virtual void DisableAllVirtualMouse() =>
			_virtualMouseContainer.DisableAll();


		public virtual void SetVirtualMouseMoveSensitivity(int playerIndex, float moveSensitivity) =>
			_virtualMouseContainer.SetMoveSensitivity(playerIndex, moveSensitivity);

		public virtual void SetVirtualMouseMoveSensitivity(float moveSensitivity) =>
			_virtualMouseContainer.SetMoveSensitivity(moveSensitivity);


		public virtual void SetVirtualMouseScrollSensitivity(int playerIndex, float scrollSensitivity) =>
			_virtualMouseContainer.SetScrollSensitivity(playerIndex, scrollSensitivity);

		public virtual void SetVirtualMouseScrollSensitivity(float scrollSensitivity) =>
			_virtualMouseContainer.SetScrollSensitivity(scrollSensitivity);


		public virtual void SetVirtualMouseScreenPadding(int playerIndex, bool isByRatio, RectOffset padding) =>
			_virtualMouseContainer.SetScreenPadding(playerIndex, isByRatio, padding);

		public virtual void SetVirtualMouseScreenPadding(bool isByRatio, RectOffset padding) =>
			_virtualMouseContainer.SetScreenPadding(isByRatio, padding);


		public virtual void SetVirtualMouseScreenPadding(int playerIndex, RectOffset padding) =>
			_virtualMouseContainer.SetScreenPadding(playerIndex, padding);

		public virtual void SetVirtualMouseScreenPadding(RectOffset padding) =>
			_virtualMouseContainer.SetScreenPadding(padding);

		#endregion

		// PROTECT METHOD: --------------------------------------------------------------------

		protected virtual void CreatePlayerInputManager()
		{
			if (_playerInputManager != null)
				DestroyPlayerInputManager();

			_playerInputManager = Object.Instantiate(_inputModuleConfig.PlayerInputManagerPrefab, _root).GetComponent<PlayerInputManager>();
			DisableJoining();
			_playerInputManager.onPlayerJoined += OnPlayerJoinedImp;
			_playerInputManager.onPlayerLeft += OnPlayerLeftImp;
			EnableJoining();
		}

		protected virtual void DestroyPlayerInputManager()
		{
			if (_playerInputManager is null)
				return;

			foreach (var playerInput in _playerInputs.ToArray())
				OnPlayerLeftImp(playerInput);

			var playerInputManager = _playerInputManager;
			_playerInputManager = null;
			Object.Destroy(playerInputManager.gameObject);
		}

		protected virtual void CreatePlayerInput()
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

			_virtualMouseContainer.Create(_playerInput);

			OnControlsChangedImp_SinglePlayer(_playerInput, false);
			_playerInput.onControlsChanged += OnControlsChangedImp_SinglePlayer;

			SwitchCurrentActionMap(_playerInput.playerIndex);
		}

		protected virtual void DestroyPlayerInput()
		{
			if (_playerInput is null)
				return;

			var playerInput = _playerInput;
			_playerInput = null;

			_playerInputs.Remove(playerInput);
			OnPlayerLeft?.Invoke(playerInput);

			_virtualMouseContainer.Destroy(playerInput.playerIndex);

			Object.Destroy(playerInput.gameObject);
		}

		protected virtual void SwitchCurrentActionMap(int playerIndex) =>
			SwitchActionMap(playerIndex, CurrentActionMapDataId);

		protected virtual void SwitchActionMap(int playerIndex, string actionMapDataId)
		{
			if (!TryGetPlayerInput(playerIndex, out var playerInput))
				return;

			playerInput.SwitchCurrentActionMap(actionMapDataId);
		}

		protected virtual void OnPlayerJoinedImp(PlayerInput playerInput)
		{
			if (_playerInputManager.playerCount >= MaxPlayerCount)
				_playerInputManager.DisableJoining();

			// cant change playerInput.actions. PlayerInputManager will destroy playerInput.
			// playerInput.actions = GetAndCloneDefaultAsset();

			playerInput.actions.Disable();
			var timerId = _timerModule.AddOnceTimer(_inputModuleConfig.JoinedWaitingTime, _ =>
			{
				if (IsEnableInput)
					playerInput.actions.Enable();

				_timerIdMapByIndex.Remove(playerInput.playerIndex);
			});

			_timerIdMapByIndex.Add(playerInput.playerIndex, timerId);

			playerInput.transform.SetParent(_playerInputManager.transform);
			_playerInputs.Add(playerInput);
			OnPlayerJoined?.Invoke(playerInput);

			var playerIndex = playerInput.playerIndex;
			_virtualMouseContainer.Create(playerInput);

			OnControlsChangedImp(playerInput, false);
			playerInput.onControlsChanged += OnControlsChangedImp;

			SwitchCurrentActionMap(playerIndex);
		}

		protected virtual void OnPlayerLeftImp(PlayerInput playerInput)
		{
			if (IsEnableJoining && _playerInputManager.playerCount < MaxPlayerCount)
				_playerInputManager.EnableJoining();

			if (_timerIdMapByIndex.TryGetValue(playerInput.playerIndex, out var timerId))
			{
				_timerModule.RemoveTimer(timerId);
				_timerIdMapByIndex.Remove(playerInput.playerIndex);
			}

			_virtualMouseContainer.Destroy(playerInput.playerIndex);

			_playerInputs.Remove(playerInput);
			OnPlayerLeft?.Invoke(playerInput);
		}

		protected virtual void OnControlsChangedImp_SinglePlayer(PlayerInput playerInput) =>
			OnControlsChangedImp(playerInput, true);

		protected virtual void OnControlsChangedImp_SinglePlayer(PlayerInput playerInput, bool isSyncPosition)
		{
			OnControlsChangedImp(playerInput, isSyncPosition);
			_previousControlScheme = playerInput.currentControlScheme;
		}

		protected virtual void OnControlsChangedImp(PlayerInput playerInput) =>
			OnControlsChangedImp(playerInput, true);


		protected virtual void OnControlsChangedImp(PlayerInput playerInput, bool isSyncPosition)
		{
			OnControlsChanged?.Invoke(playerInput);

			var playerIndex = playerInput.playerIndex;

			// If player is single player or first player, sync with hardware mouse.
			if (!IsSinglePlayer && playerIndex != 0)
				return;

			var controllerSchemeName = _inputModuleConfig.ControllerSchemeName;
			if (playerInput.currentControlScheme == controllerSchemeName)
			{
				if (isSyncPosition)
				{
					if (_virtualMouseContainer.CanEnableVirtualMouse)
						_virtualMouseContainer.Enable(playerIndex);
					_virtualMouseContainer.SyncVirtualMouseToHardwareMousePosition(playerIndex);
				}

				if (_inputModuleConfig.IsAutoHideHardwareCursor)
					HideHardwareCursor();
			}
			else if (playerInput.currentControlScheme != controllerSchemeName)
			{
				if (isSyncPosition)
				{
					_virtualMouseContainer.SyncHardwareMouseToVirtualMousePosition(playerIndex);
					_virtualMouseContainer.Disable(playerIndex);
				}

				ShowHardwareCursor();
			}
		}

		protected virtual bool TryGetPreviousControlScheme(out string previousControlScheme)
		{
			if (string.IsNullOrEmpty(_previousControlScheme) || string.IsNullOrWhiteSpace(_previousControlScheme))
			{
				previousControlScheme = string.Empty;
				return false;
			}

			previousControlScheme = _previousControlScheme;
			return true;
		}

		protected virtual PlayerInput GetPlayerInputPrefab() =>
			_inputModuleConfig.PlayerInputManagerPrefab.GetComponent<PlayerInputManager>().playerPrefab.GetComponent<PlayerInput>();


		protected virtual void CheckAutoHideEventSystem()
		{
			if (!CanEnableEventSystem || !IsEnableEventSystem)
				return;

			var isAnyCursorExist = IsHardwareCursorVisible || InputSystem.devices.Any(device => device is Mouse { native: false, enabled: true });

			if (isAnyCursorExist && !_eventSystem.activeSelf)
				_eventSystem.SetActive(true);
			else if (!isAnyCursorExist && _eventSystem.activeSelf)
				_eventSystem.SetActive(false);
		}

		protected virtual void CheckAutoHideHardwareCursorTime(float deltaTime)
		{
			if (!CanEnableEventSystem || !IsEnableEventSystem || !_inputModuleConfig.IsAutoHideHardwareCursor || !InputSystemUtils.TryGetHardwareMouse(out var hardwareMouse))
				return;

			if (hardwareMouse.CheckMouseHasAnyInput())
			{
				// Show mouse and reset timer if mouse has any input
				ShowHardwareCursor();
			}
			else if (IsHardwareCursorVisible)
			{
				// Count down the timer if mouse has no input
				_autoHideHardwareCursorTimer -= deltaTime;
				if (_autoHideHardwareCursorTimer <= 0)
					HideHardwareCursor();
			}
		}

		protected virtual void ShowHardwareCursor()
		{
			if (!Cursor.visible)
				Cursor.visible = true;

			IsHardwareCursorVisible = true;
			_autoHideHardwareCursorTimer = _inputModuleConfig.AutoHideHardwareCursorTime;
		}

		protected virtual void HideHardwareCursor()
		{
			if (Cursor.visible)
				Cursor.visible = false;

			IsHardwareCursorVisible = false;
		}

		protected virtual string GetVirtualMouseMoveActionPathImp(int playerIndex, string currentActionMapDataId) =>
			GetVirtualMouseMoveActionPath?.Invoke(playerIndex, currentActionMapDataId);

		protected virtual string GetVirtualMouseLeftButtonActionPathImp(int playerIndex, string currentActionMapDataId) =>
			GetVirtualMouseLeftButtonActionPath?.Invoke(playerIndex, currentActionMapDataId);

		protected virtual string GetVirtualMouseRightButtonActionPathImp(int playerIndex, string currentActionMapDataId) =>
			GetVirtualMouseRightButtonActionPath?.Invoke(playerIndex, currentActionMapDataId);

		protected virtual string GetVirtualMouseScrollActionPathImp(int playerIndex, string currentActionMapDataId) =>
			GetVirtualMouseScrollActionPath?.Invoke(playerIndex, currentActionMapDataId);
	}
}