using System;
using System.Collections.Generic;
using System.Linq;
using CizaPageModule;
using CizaPageModule.Implement;
using CizaUniTask;
using UnityEngine;
using UnityEngine.Assertions;

namespace CizaOptionModule
{
    public abstract class OptionModulePage : Page, IOptionModulePage, IInitializable, IShowingPrepare, IShowingStart, IShowingAnimated, IShowingAnimatedImmediately, ITickable, IHidingStart, IHidingAnimated, IReleasable
    {
        [SerializeField]
        protected Transform _parentTransform;

        protected readonly Dictionary<int, bool> _isFromPointerEnterMapByPlayerIndex = new Dictionary<int, bool>();
        protected readonly SelectOptionLogic _selectOptionLogic = new SelectOptionLogic();

        protected IOptionView OptionView { get; private set; }
        protected Dictionary<int, Vector2Int> OnShowingStartCoordinateMapByPlayerIndex { get; private set; }


        protected bool IsImmediately { get; private set; }
        protected bool IsAutoTurnOffIsNew { get; private set; }
        protected bool LocalIsAutoTurnOffIsNew { get; private set; }

        public event Action<int, string, string> OnSelect;
        public event Action<int, string, bool> OnConfirm;

        public int PageIndex { get; private set; }

        public int MaxColumnIndex => _selectOptionLogic.MaxColumnLength - 1;
        public int MaxRowIndex => _selectOptionLogic.MaxRowLength - 1;

        public virtual UniTask InitializeAsync(params object[] parameters)
        {
            var optionModule = parameters[0] as OptionModule;
            var optionDefaultPlayerIndex = (int)parameters[1];

            var optionModulePageInfo = parameters[2] as IOptionModulePageInfo;
            Assert.IsNotNull(optionModulePageInfo, $"[{GetType().Name}::Initialize] OptionModulePageInfo is not found.");

            PageIndex = int.Parse(optionModulePageInfo.PageIndexString);

            var optionViewGameObject = Instantiate(optionModulePageInfo.OptionViewPrefab, _parentTransform);
            OptionView = optionViewGameObject.GetComponent<IOptionView>();

            var optionInfos = parameters[3] as IOptionInfo[];
            OptionView.OptionsIncludeNull.InitializeOptions(optionModule, optionDefaultPlayerIndex, optionModulePageInfo.OptionKeys, optionInfos, OnConfirmImp, null, GetType().Name);

            _selectOptionLogic.Initialize(OptionView.OptionColumns, OptionView.Options, OptionView.ColumnInfo, OptionView.RowInfo);
            _selectOptionLogic.OnRemovePlayer += OnRemovePlayer;
            _selectOptionLogic.OnSetCurrentCoordinate += OnSetCurrentCoordinate;

            OptionView.Initialize();
            return UniTask.CompletedTask;
        }

        public virtual UniTask ShowingPrepareAsync(params object[] parameters)
        {
            OnShowingStartCoordinateMapByPlayerIndex = parameters[0] as Dictionary<int, Vector2Int>;
            IsAutoTurnOffIsNew = (bool)parameters[1];
            return UniTask.CompletedTask;
        }

        public virtual void ShowingStart()
        {
            IsImmediately = true;
            OptionView.PlayShowStartAndPause();

            OptionView.UnSelectAll();
            LocalIsAutoTurnOffIsNew = true;

            foreach (var playerIndex in _selectOptionLogic.PlayerIndexList)
                _selectOptionLogic.TrySetCurrentCoordinate(playerIndex, OnShowingStartCoordinateMapByPlayerIndex[playerIndex]);

            IsImmediately = false;
        }

        public virtual UniTask PlayShowingAnimationAsync() =>
            OptionView.PlayShowAsync();

        public virtual void PlayShowingAnimationImmediately() =>
            OptionView.PlayShowComplete();

        public void Tick(float deltaTime) =>
            OptionView.Tick(deltaTime);

        public virtual void HidingStart() =>
            LocalIsAutoTurnOffIsNew = false;

        public virtual UniTask PlayHidingAnimationAsync() =>
            OptionView.PlayHideAsync();

        public virtual void Release()
        {
            OptionView.Release();
            _selectOptionLogic.OnRemovePlayer -= OnRemovePlayer;
            _selectOptionLogic.OnSetCurrentCoordinate -= OnSetCurrentCoordinate;
        }

        public Option[] GetAllOptions() =>
            OptionView.Options.ToArray();

        public bool TryGetOption(string optionKey, out Option option)
        {
            option = OptionView.Options.FirstOrDefault(m_option => m_option.Key == optionKey);
            return option != null;
        }

        public bool TryGetCurrentCoordinate(int playerIndex, out Vector2Int currentCoordinate) =>
            _selectOptionLogic.TryGetCurrentCoordinate(playerIndex, out currentCoordinate);

        public bool TryGetCurrentOptionKey(int playerIndex, out string currentOptionKey) =>
            _selectOptionLogic.TryGetCurrentOptionKey(playerIndex, out currentOptionKey);

        public void AddPlayer(int playerIndex)
        {
            _isFromPointerEnterMapByPlayerIndex.Add(playerIndex, false);
            _selectOptionLogic.AddPlayer(playerIndex);
        }

        public void RemovePlayer(int playerIndex)
        {
            _selectOptionLogic.RemovePlayer(playerIndex);
            _isFromPointerEnterMapByPlayerIndex.Remove(playerIndex);
        }


        public bool TrySetCurrentCoordinate(int playerIndex, Vector2Int coordinate) =>
            _selectOptionLogic.TrySetCurrentCoordinate(playerIndex, coordinate);

        public bool TrySetCurrentCoordinate(int playerIndex, string optionKey, bool isFromPointerEnter)
        {
            _isFromPointerEnterMapByPlayerIndex[playerIndex] = isFromPointerEnter;
            return _selectOptionLogic.TrySetCurrentCoordinate(playerIndex, optionKey);
        }

        public bool TryConfirm(int playerIndex)
        {
            if (!_selectOptionLogic.TryGetCurrentCoordinate(playerIndex, out var currentCoordinate))
                return false;

            if (!_selectOptionLogic.TryGetOption(currentCoordinate, out var option) || !option.CanConfirm)
                return false;

            return option.TryConfirm(playerIndex);
        }

        public virtual bool TryMoveToLeft(int playerIndex) =>
            _selectOptionLogic.TryMoveToLeft(playerIndex, true);

        public virtual bool TryMoveToRight(int playerIndex) =>
            _selectOptionLogic.TryMoveToRight(playerIndex, true);

        public virtual bool TryMoveToUp(int playerIndex) =>
            _selectOptionLogic.TryMoveToUp(playerIndex, true);

        public virtual bool TryMoveToDown(int playerIndex) =>
            _selectOptionLogic.TryMoveToDown(playerIndex, true);

        protected virtual void OnConfirmImp(int playerIndex, string optionKey, bool isUnlock) =>
            OnConfirm?.Invoke(playerIndex, optionKey, isUnlock);

        protected virtual void OnRemovePlayer(int playerIndex, Vector2Int coordinate) =>
            Unselect(coordinate);

        protected virtual void OnSetCurrentCoordinate(int playerIndex, Vector2Int previousCoordinate, Option previousOption, Vector2Int currentCoordinate, Option currentOption)
        {
            OptionView.SetCurrentCoordinate(playerIndex, previousCoordinate, previousOption, currentCoordinate, currentOption, IsImmediately, _isFromPointerEnterMapByPlayerIndex[playerIndex]);

            Unselect(previousCoordinate);

            currentOption.Select(IsAutoTurnOffIsNew && LocalIsAutoTurnOffIsNew);
            OnSelect?.Invoke(playerIndex, previousOption != null ? previousOption.Key : string.Empty, currentOption != null ? currentOption.Key : string.Empty);

            _isFromPointerEnterMapByPlayerIndex[playerIndex] = false;
        }

        private void Unselect(Vector2Int coordinate)
        {
            if (_selectOptionLogic.CheckIsAnyPlayerOnCoordinate(coordinate) || !_selectOptionLogic.TryGetOption(coordinate, out var option))
                return;

            option.Unselect();
        }
    }
}