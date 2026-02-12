using CizaCore;
using CizaUniTask;
using UnityEngine;

namespace CizaOptionModule
{
	public interface IOptionView
	{
		IOptionColumn[] OptionColumns { get; }
		Option[] Options { get; }
		Option[] OptionsIncludeNull { get; }

		IColumnInfo ColumnInfo { get; }
		IRowInfo RowInfo { get; }

		void Initialize();

		void UnSelectAll();

		void PlayShowStartAndPause();

		UniTask PlayShowAsync();

		void PlayShowComplete();

		void Tick(float deltaTime);

		UniTask PlayHideAsync();

		void Release();

		void SetCurrentCoordinate(int playerIndex, Vector2Int previousCoordinate, Option previousOption, Vector2Int currentCoordinate, Option currentOption, bool isImmediately, bool isFromPointerEnter);
	}
}