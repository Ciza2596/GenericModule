using System;
using UnityEngine;

namespace CizaInputModule
{
	public interface IVirtualMouse : IVirtualMouseReadModel
	{
		// PlayerIndex, ActionMapDataId, Returns Path
		event Func<int, string, string> GetMoveActionPath;

		// PlayerIndex, ActionMapDataId, Returns Path
		event Func<int, string, string> GetLeftButtonActionPath;

		// PlayerIndex, ActionMapDataId, Returns Path
		event Func<int, string, string> GetRightButtonActionPath;

		// PlayerIndex, ActionMapDataId, Returns Path
		event Func<int, string, string> GetScrollActionPath;


		void Initialize(float moveSensitivity, float scrollSensitivity, bool isScreenPaddingByRatio, RectOffset screenPadding);
		void Release();

		void Enable();
		void Disable();

		void SetMoveSensitivity(float sensitivity);
		void SetScrollSensitivity(float sensitivity);

		void SetScreenPadding(bool isByRatio, RectOffset padding);
		void AnchorVirtualMouseToPosition(Vector2 position);
	}
}