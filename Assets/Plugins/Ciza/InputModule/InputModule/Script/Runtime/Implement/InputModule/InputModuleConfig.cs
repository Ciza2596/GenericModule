using System;
using UnityEngine;

namespace CizaInputModule.Implement
{
	[CreateAssetMenu(fileName = "InputModuleConfig", menuName = "Ciza/InputModule/InputModuleConfig", order = -1)]
	public class InputModuleConfig : ScriptableObject, IInputModuleConfig
	{
		// VARIABLE: -----------------------------------------------------------------------------

		[SerializeField]
		private string _rootName;

		[SerializeField]
		private bool _isDontDestroyOnLoad;

		[Space]
		[SerializeField]
		private GameObject _eventSystemPrefab;

		[SerializeField]
		private bool _canEnableEventSystem;

		[SerializeField]
		private bool _isDefaultEnableEventSystem;

		[Space]
		[SerializeField]
		private bool _isAutoHideHardwareCursor;

		[SerializeField]
		private float _autoHideHardwareCursorTime;

		[Space]
		[SerializeField]
		private GameObject _playerInputManagerPrefab;

		[SerializeField]
		private float _joinedWaitingTime;

		[Space]
		[SerializeField]
		private string _defaultActionMapDataId;

		[SerializeField]
		private string _disableActionMapDataId;

		[Space]
		[SerializeField]
		private string _controllerSchemeName;


		[Space]
		[Space]
		[SerializeField]
		private VirtualMouseContainerConfigEnabler _canEnableVirtualMouse;


		// PUBLIC VARIABLE: ---------------------------------------------------------------------

		public virtual string RootName => _rootName;
		public virtual bool IsDontDestroyOnLoad => _isDontDestroyOnLoad;

		public virtual GameObject EventSystemPrefab => _eventSystemPrefab;
		public virtual bool CanEnableEventSystem => _canEnableEventSystem;
		public virtual bool IsDefaultEnableEventSystem => _isDefaultEnableEventSystem;

		public virtual bool IsAutoHideHardwareCursor => _isAutoHideHardwareCursor;
		public virtual float AutoHideHardwareCursorTime => _autoHideHardwareCursorTime;

		public virtual GameObject PlayerInputManagerPrefab => _playerInputManagerPrefab;
		public virtual float JoinedWaitingTime => _joinedWaitingTime;

		public virtual string DefaultActionMapDataId => _defaultActionMapDataId;
		public virtual string DisableActionMapDataId => _disableActionMapDataId;

		public virtual string ControllerSchemeName => _controllerSchemeName;


		#region VirtualMouse

		public virtual bool CanEnableVirtualMouse => _canEnableVirtualMouse.IsEnable;

		public virtual Vector2Int ReferenceResolution => _canEnableVirtualMouse.Value.ReferenceResolution;

		public virtual float MoveSensitivity => _canEnableVirtualMouse.Value.MoveSensitivity;
		public virtual float ScrollSensitivity => _canEnableVirtualMouse.Value.ScrollSensitivity;

		public virtual bool IsScreenPaddingByRatio => _canEnableVirtualMouse.Value.IsScreenPaddingByRatio;
		public virtual RectOffset ScreenPadding => _canEnableVirtualMouse.Value.ScreenPadding;

		public virtual GameObject CanvasPrefab => _canEnableVirtualMouse.Value.CanvasPrefab;

		public virtual bool TryGetInfo(int playerIndex, out IVirtualMouseInfo info) =>
			_canEnableVirtualMouse.Value.TryGetInfo(playerIndex, out info);

		#endregion


		// CONSTRUCTOR: ------------------------------------------------------------------------

		public virtual void Reset()
		{
			_rootName = "[InputModule]";
			_isDontDestroyOnLoad = true;

			_eventSystemPrefab = null;
			_canEnableEventSystem = true;
			_isDefaultEnableEventSystem = false;

			_isAutoHideHardwareCursor = true;
			_autoHideHardwareCursorTime = 3;

			_playerInputManagerPrefab = null;
			_joinedWaitingTime = 0.25f;

			_defaultActionMapDataId = "None";
			_disableActionMapDataId = "Disable";

			_controllerSchemeName = "Controller";

			_canEnableVirtualMouse = new VirtualMouseContainerConfigEnabler();
		}
	}
}