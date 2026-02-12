using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;
using UnityEngine.Scripting;
using Object = UnityEngine.Object;

namespace CizaInputModule
{
	public class VirtualMouse : IVirtualMouse
	{
		// VARIABLE: -----------------------------------------------------------------------------

		protected readonly IVirtualMouseInfo _virtualMouseInfo;
		protected readonly Canvas _canvas;
		protected readonly PlayerInput _playerInput;
		protected readonly Vector2Int _referenceResolution;

		protected RectTransform _cursor;
		protected Mouse _mouse;

		protected double _lastInputTime;
		protected Vector2 _lastStickValue;

		protected InputAction _moveAction;
		protected InputAction _leftButtonAction;
		protected InputAction _rightButtonAction;
		protected InputAction _scrollAction;

		protected virtual bool IsInitialized => _mouse != null;


		protected virtual RectTransform CanvasRectTransform => _canvas.GetComponent<RectTransform>();
		protected virtual Camera CanvasCamera => _canvas.worldCamera;

		protected virtual float ScreenHorizontalRatio => (float)Screen.width / _referenceResolution.x;
		protected virtual float ScreenVerticalRatio => (float)Screen.height / _referenceResolution.y;

		// EVENT: ---------------------------------------------------------------------------------

		// PlayerIndex, CurrentActionMapDataId, Returns Path
		public event Func<int, string, string> GetMoveActionPath;

		// PlayerIndex, CurrentActionMapDataId, Returns Path
		public event Func<int, string, string> GetLeftButtonActionPath;

		// PlayerIndex, CurrentActionMapDataId, Returns Path
		public event Func<int, string, string> GetRightButtonActionPath;

		// PlayerIndex, CurrentActionMapDataId, Returns Path
		public event Func<int, string, string> GetScrollActionPath;


		// PUBLIC VARIABLE: ---------------------------------------------------------------------

		public virtual int PlayerIndex => _playerInput?.playerIndex ?? -1;
		public virtual string VirtualMouseName => $"VirtualMouse_{PlayerIndex:D2}";

		public virtual bool IsEnable => _mouse?.enabled ?? false;

		public virtual Mouse Mouse => _mouse;
		public virtual string CurrentActionMapDataId { get; protected set; }

		public virtual float MoveSensitivity { get; protected set; }
		public virtual float ScrollSensitivity { get; protected set; }

		public virtual bool IsScreenPaddingByRatio { get; protected set; }
		public virtual RectOffset ScreenPadding { get; protected set; }


		// CONSTRUCTOR: ------------------------------------------------------------------------

		[Preserve]
		public VirtualMouse(IVirtualMouseInfo virtualMouseInfo, Canvas canvas, PlayerInput playerInput, Vector2Int referenceResolution)
		{
			_virtualMouseInfo = virtualMouseInfo;
			_canvas = canvas;
			_playerInput = playerInput;
			_referenceResolution = referenceResolution;
		}

		// LIFECYCLE METHOD: ------------------------------------------------------------------

		public virtual void Initialize(float moveSensitivity, float scrollSensitivity, bool isScreenPaddingByRatio, RectOffset screenPadding)
		{
			if (IsInitialized)
				return;

			var virtualMouseName = VirtualMouseName;

			_cursor = Object.Instantiate(_virtualMouseInfo.BodyPrefab, _canvas.transform).GetComponent<RectTransform>();
			_cursor.name = virtualMouseName;

			if (_mouse == null)
				_mouse = InputSystem.GetDevice<Mouse>(virtualMouseName) ?? InputSystem.AddDevice<Mouse>(virtualMouseName);
			else if (!_mouse.added)
				InputSystem.AddDevice(_mouse);

			InputUser.PerformPairingWithDevice(_mouse, _playerInput.user);
			InputSystem.onAfterUpdate += OnUpdateMotion;
			_playerInput.onControlsChanged += OnActionMapChanged;

			SetMoveSensitivity(moveSensitivity);
			SetScrollSensitivity(scrollSensitivity);
			SetScreenPadding(isScreenPaddingByRatio, screenPadding);
		}

		public virtual void Release()
		{
			if (!IsInitialized)
				return;

			_playerInput.user.UnpairDevice(_mouse);
			InputSystem.onAfterUpdate -= OnUpdateMotion;
			_playerInput.onControlsChanged -= OnActionMapChanged;

			if (_mouse is { added: true })
				InputSystem.RemoveDevice(_mouse);

			if (_cursor != null)
				Object.Destroy(_cursor.gameObject);

			_lastInputTime = 0;
			_lastStickValue = Vector2.zero;
		}


		// PUBLIC METHOD: ----------------------------------------------------------------------


		public virtual void Enable() =>
			SetIsEnabled(true);

		public virtual void Disable() =>
			SetIsEnabled(false);

		public virtual void SetMoveSensitivity(float sensitivity)
		{
			if (!IsInitialized)
				return;

			MoveSensitivity = sensitivity;
		}

		public virtual void SetScrollSensitivity(float sensitivity)
		{
			if (!IsInitialized)
				return;

			ScrollSensitivity = sensitivity;
		}

		public virtual void SetScreenPadding(bool isByRatio, RectOffset padding)
		{
			if (!IsInitialized)
				return;

			IsScreenPaddingByRatio = isByRatio;
			ScreenPadding = padding;
		}

		public virtual void AnchorVirtualMouseToPosition(Vector2 position)
		{
			if (!IsInitialized)
				return;

			InputState.Change(_mouse.position, position);

			if (_cursor == null)
				return;

			RectTransformUtility.ScreenPointToLocalPointInRectangle(CanvasRectTransform, position, CanvasCamera, out var anchoredPosition);
			_cursor.anchoredPosition = anchoredPosition;
		}

		// PROTECT METHOD: --------------------------------------------------------------------

		protected virtual void SetIsEnabled(bool isEnabled)
		{
			if (!IsInitialized)
				return;

			if (_mouse is not { added: true })
				return;

			if (isEnabled && !_mouse.enabled)
				InputSystem.EnableDevice(_mouse);
			else if (!isEnabled && _mouse.enabled)
				InputSystem.DisableDevice(_mouse);

			if (_cursor != null)
				_cursor.gameObject.SetActive(isEnabled);
		}


		protected virtual void OnActionMapChanged(PlayerInput playerInput)
		{
			var actionMapDataId = playerInput.currentActionMap.name;

			if (CurrentActionMapDataId == actionMapDataId)
				return;

			if (!string.IsNullOrEmpty(CurrentActionMapDataId))
			{
				UnregisterButtonActionCallback(_leftButtonAction);
				UnregisterButtonActionCallback(_rightButtonAction);
			}

			CurrentActionMapDataId = actionMapDataId;
			RefreshActions();
			RegisterButtonActionCallback(_leftButtonAction);
			RegisterButtonActionCallback(_rightButtonAction);

			return;

			void RegisterButtonActionCallback(InputAction action)
			{
				if (action == null) return;
				action.started += OnButtonActionTriggered;
				action.canceled += OnButtonActionTriggered;
			}

			void UnregisterButtonActionCallback(InputAction action)
			{
				if (action == null) return;
				action.started -= OnButtonActionTriggered;
				action.canceled -= OnButtonActionTriggered;
			}
		}

		protected virtual void OnUpdateMotion()
		{
			if (_mouse is not { added: true, enabled: true })
				return;

			var moveAction = _moveAction;
			if (moveAction == null)
				return;
			var moveValue = moveAction.ReadValue<Vector2>();

			if (Mathf.Approximately(0, moveValue.x) && Mathf.Approximately(0, moveValue.y))
			{
				_lastInputTime = 0;
				_lastStickValue = Vector2.zero;
			}
			else
			{
				var currentTime = InputState.currentTime;

				if (Mathf.Approximately(0, _lastStickValue.x) && Mathf.Approximately(0, _lastStickValue.y))
					_lastInputTime = currentTime;

				var deltaTime = (float)(currentTime - _lastInputTime);
				var delta = new Vector2(moveValue.x * ScreenHorizontalRatio, moveValue.y * ScreenVerticalRatio) * MoveSensitivity * deltaTime;

				var currentPosition = _mouse.position.value;
				var newPosition = currentPosition + delta;

				if (_canvas != null)
				{
					var screenPadding = IsScreenPaddingByRatio ? CreateScreenPaddingByRatio(ScreenPadding) : ScreenPadding;
					newPosition.x = Mathf.Clamp(newPosition.x, screenPadding.left, Screen.width - screenPadding.right);
					newPosition.y = Mathf.Clamp(newPosition.y, screenPadding.bottom, Screen.height - screenPadding.top);
				}

				InputState.Change(_mouse.delta, delta);
				AnchorVirtualMouseToPosition(newPosition);

				_lastStickValue = moveValue;
				_lastInputTime = currentTime;
			}

			var scrollAction = _scrollAction;
			if (scrollAction == null)
				return;
			var scrollValue = scrollAction.ReadValue<Vector2>();
			scrollValue.x *= ScrollSensitivity;
			scrollValue.y *= ScrollSensitivity;

			InputState.Change(_mouse.scroll, scrollValue);
		}

		protected virtual void OnButtonActionTriggered(InputAction.CallbackContext context)
		{
			if (_mouse is not { added: true, enabled: true })
				return;

			var action = context.action;
			if (action == null)
				return;

			MouseButton? button = null;
			if (action == _leftButtonAction)
				button = MouseButton.Left;
			else if (action == _rightButtonAction)
				button = MouseButton.Right;

			if (button == null)
				return;

			var isPressed = context.control.IsPressed();
			_mouse.CopyState<MouseState>(out var mouseState);
			mouseState.WithButton(button.Value, isPressed);

			InputState.Change(_mouse, mouseState);
		}

		protected virtual RectOffset CreateScreenPaddingByRatio(RectOffset basePadding)
		{
			var horizontalRatio = ScreenHorizontalRatio;
			var verticalRatio = ScreenVerticalRatio;

			var left = Mathf.RoundToInt(horizontalRatio * basePadding.left);
			var right = Mathf.RoundToInt(horizontalRatio * basePadding.right);
			var top = Mathf.RoundToInt(verticalRatio * basePadding.top);
			var bottom = Mathf.RoundToInt(verticalRatio * basePadding.bottom);

			return new RectOffset(left, right, top, bottom);
		}


		protected virtual void RefreshActions()
		{
			_moveAction = _playerInput?.actions?.FindAction(GetMoveActionPathImp(PlayerIndex, CurrentActionMapDataId));
			_leftButtonAction = _playerInput?.actions?.FindAction(GetLeftButtonActionPathImp(PlayerIndex, CurrentActionMapDataId));
			_rightButtonAction = _playerInput?.actions?.FindAction(GetRightButtonActionPathImp(PlayerIndex, CurrentActionMapDataId));
			_scrollAction = _playerInput?.actions?.FindAction(GetScrollActionPathImp(PlayerIndex, CurrentActionMapDataId));
		}

		protected virtual string GetMoveActionPathImp(int playerIndex, string currentActionMapDataId) =>
			GetMoveActionPath?.Invoke(playerIndex, currentActionMapDataId) ?? string.Empty;

		protected virtual string GetLeftButtonActionPathImp(int playerIndex, string currentActionMapDataId) =>
			GetLeftButtonActionPath?.Invoke(playerIndex, currentActionMapDataId) ?? string.Empty;

		protected virtual string GetRightButtonActionPathImp(int playerIndex, string currentActionMapDataId) =>
			GetRightButtonActionPath?.Invoke(playerIndex, currentActionMapDataId) ?? string.Empty;

		protected virtual string GetScrollActionPathImp(int playerIndex, string currentActionMapDataId) =>
			GetScrollActionPath?.Invoke(playerIndex, currentActionMapDataId) ?? string.Empty;
	}
}