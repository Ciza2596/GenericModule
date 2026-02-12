using UnityEngine;

namespace CizaInputModule
{
	public interface IVirtualMouseContainerConfig
	{
		Vector2Int ReferenceResolution { get; }
		float MoveSensitivity { get; }
		float ScrollSensitivity { get; }

		bool IsScreenPaddingByRatio { get; }
		RectOffset ScreenPadding { get; }

		GameObject CanvasPrefab { get; }
		bool TryGetInfo(int playerIndex, out IVirtualMouseInfo info);
	}
}