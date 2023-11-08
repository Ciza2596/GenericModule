using System;
using UnityEngine;

namespace CizaOptionModule
{
	public interface IOptionModulePage
	{
		event Action<string, string> OnSelect;
		event Action<string, bool>   OnConfirm;

		string OptionKey { get; }

		int        PageIndex         { get; }
		Vector2Int CurrentCoordinate { get; }

		int MaxColumnIndex { get; }
		int MaxRowIndex    { get; }

		bool TryGetOption(string optionKey, out Option option);

		bool TrySetCurrentCoordinate(Vector2Int coordinate);

		bool TryConfirm();

		bool TryMoveToLeft();

		bool TryMoveToRight();

		bool TryMoveToUp();

		bool TryMoveToDown();
	}
}
