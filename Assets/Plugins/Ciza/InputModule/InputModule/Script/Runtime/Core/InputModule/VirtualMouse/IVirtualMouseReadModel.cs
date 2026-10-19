using UnityEngine;
using UnityEngine.InputSystem;

namespace CizaInputModule
{
	public interface IVirtualMouseReadModel
	{
		int PlayerIndex { get; }
		string VirtualMouseName { get; }

		bool IsEnable { get; }

		Mouse Mouse { get; }
		string ActionMapDataId { get; }

		float MoveSensitivity { get; }
		float ScrollSensitivity { get; }

		bool IsScreenPaddingByRatio { get; }
		RectOffset ScreenPadding { get; }
	}
}