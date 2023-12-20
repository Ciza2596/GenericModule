using System;
using CizaCore;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CizaOptionModule.Implement
{
    public class OptionView : MonoBehaviour, IOptionView
    {
        [SerializeField]
        private CollectionSettings _collectionSettings;

        [Space]
        [SerializeField]
        private bool _isUseAnim;

        [SerializeField]
        private AnimSettings _animSettings;

        [Serializable]
        private class CollectionSettings : OptionSettings<Option>
        {
            [SerializeField]
            private ColumnInfoImp _columnInfo;

            [SerializeField]
            private RowInfoImp _rowInfo;

            public IColumnInfo ColumnInfo => _columnInfo;
            public IRowInfo RowInfo => _rowInfo;
        }

        public IOptionColumn[] OptionColumns => _collectionSettings.OptionColumns;
        public Option[] Options => _collectionSettings.Options;
        public Option[] OptionsIncludeNull => _collectionSettings.OptionsIncludeNull;
        public IColumnInfo ColumnInfo => _collectionSettings.ColumnInfo;
        public IRowInfo RowInfo => _collectionSettings.RowInfo;

        public void UnSelectAll()
        {
            foreach (var option in Options)
                option.Unselect();
        }

        public void PlayShowStartAndPause()
        {
            if (!_isUseAnim || !_animSettings.IsActiveAndEnabled)
                return;

            _animSettings.PlayShowStartAndPause();
        }

        public UniTask PlayShowAsync()
        {
            if (!_isUseAnim || !_animSettings.IsActiveAndEnabled)
                return UniTask.CompletedTask;
            return _animSettings.PlayShowAsync(default);
        }

        public void PlayShowComplete()
        {
            if (!_isUseAnim || !_animSettings.IsActiveAndEnabled)
                return;
            _animSettings.PlayShowComplete();
        }

        public virtual void Tick(float deltaTime) { }

        public UniTask PlayHideAsync()
        {
            if (!_isUseAnim || !_animSettings.IsActiveAndEnabled)
                return UniTask.CompletedTask;
            return _animSettings.PlayHideAsync(default);
        }

        public virtual void OnSetCurrentCoordinate(int playerIndex, Vector2Int previousCoordinate, Option previousOption, Vector2Int currentCoordinate, Option currentOption, bool isImmediately) { }


        [Serializable]
        private class ColumnInfoImp : IColumnInfo
        {
            [SerializeField]
            private bool _isColumnCircle;

            [SerializeField]
            private bool _isNotMoveWhenNullOrDisableInColumn;

            [Space]
            [SerializeField]
            private bool _isAutoChangeRowToLeft;

            [SerializeField]
            private bool _isAutoChangeRowToRight;

            public bool IsColumnCircle => _isColumnCircle;
            public bool IsNotMoveWhenNullOrDisableInColumn => _isNotMoveWhenNullOrDisableInColumn;
            public bool IsAutoChangeRowToLeft => _isAutoChangeRowToLeft;
            public bool IsAutoChangeRowToRight => _isAutoChangeRowToRight;
        }

        [Serializable]
        private class RowInfoImp : IRowInfo
        {
            [SerializeField]
            private bool _isRowCircle;

            [SerializeField]
            private bool _isNotMoveWhenNullOrDisableInRow;

            [Space]
            [SerializeField]
            private bool _isAutoChangeColumnToUp;

            [SerializeField]
            private bool _isAutoChangeColumnToDown;

            public bool IsRowCircle => _isRowCircle;
            public bool IsNotMoveWhenNullOrDisableInRow => _isNotMoveWhenNullOrDisableInRow;
            public bool IsAutoChangeColumnToUp => _isAutoChangeColumnToUp;
            public bool IsAutoChangeColumnToDown => _isAutoChangeColumnToDown;
        }
    }
}