using UnityEngine;

namespace CizaInputModule
{
	public interface IInputModuleConfig : IVirtualMouseContainerConfig
	{
		string RootName { get; }
		bool IsDontDestroyOnLoad { get; }

		GameObject EventSystemPrefab { get; }
		bool CanEnableEventSystem { get; }
		bool IsDefaultEnableEventSystem { get; }

		bool IsAutoHideHardwareCursor { get; }
		float AutoHideHardwareCursorTime { get; }

		GameObject PlayerInputManagerPrefab { get; }
		float JoinedWaitingTime { get; }

		string DefaultActionMapDataId { get; }
		string DisableActionMapDataId { get; }

		string ControllerSchemeName { get; }
		
		bool CanEnableVirtualMouse { get; }
	}
}