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
		private bool _isAutoHideEventSystem;

		[SerializeField]
		private float _autoHideEventSystemTime;

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


		// PUBLIC VARIABLE: ---------------------------------------------------------------------

		public virtual string RootName => _rootName;
		public virtual bool IsDontDestroyOnLoad => _isDontDestroyOnLoad;

		public virtual GameObject EventSystemPrefab => _eventSystemPrefab;
		public virtual bool CanEnableEventSystem => _canEnableEventSystem;
		public virtual bool IsDefaultEnableEventSystem => _isDefaultEnableEventSystem;


		public virtual bool IsAutoHideEventSystem => _isAutoHideEventSystem;
		public virtual float AutoHideEventSystemTime => _autoHideEventSystemTime;

		public virtual GameObject PlayerInputManagerPrefab => _playerInputManagerPrefab;
		public virtual float JoinedWaitingTime => _joinedWaitingTime;

		public virtual string DefaultActionMapDataId => _defaultActionMapDataId;
		public virtual string DisableActionMapDataId => _disableActionMapDataId;


		// CONSTRUCTOR: ------------------------------------------------------------------------

		public virtual void Reset()
		{
			_rootName = "[InputModule]";
			_isDontDestroyOnLoad = true;

			_eventSystemPrefab = null;
			_canEnableEventSystem = true;
			_isDefaultEnableEventSystem = false;

			_isAutoHideEventSystem = true;
			_autoHideEventSystemTime = 3;


			_playerInputManagerPrefab = null;
			_joinedWaitingTime = 0.25f;

			_defaultActionMapDataId = "None";
			_disableActionMapDataId = "Disable";
		}
	}
}