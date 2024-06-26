using System;
using UnityEngine;

namespace CizaOptionModule
{
    public interface IOptionModulePage
    {
        event Action<int, string, string> OnSelect;
        event Action<int, string, bool> OnConfirm;

        int PageIndex { get; }

        int MaxColumnIndex { get; }
        int MaxRowIndex { get; }

        Option[] GetAllOptions();

        bool TryGetOption(string optionKey, out Option option);

        bool TryGetCurrentCoordinate(int playerIndex, out Vector2Int currentCoordinate);
        bool TryGetCurrentOptionKey(int playerIndex, out string currentOptionKey);

        void AddPlayer(int playerIndex);
        void RemovePlayer(int playerIndex);

        bool TrySetCurrentCoordinate(int playerIndex, Vector2Int coordinate);
        bool TrySetCurrentCoordinate(int playerIndex, string optionKey, bool isFromPointerEnter);

        bool TryConfirm(int playerIndex);

        bool TryMoveToLeft(int playerIndex);

        bool TryMoveToRight(int playerIndex);

        bool TryMoveToUp(int playerIndex);

        bool TryMoveToDown(int playerIndex);
    }
}