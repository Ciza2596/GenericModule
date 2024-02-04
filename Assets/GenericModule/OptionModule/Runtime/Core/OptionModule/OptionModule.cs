using System;
using System.Collections.Generic;
using System.Linq;
using CizaCore;
using CizaPageModule;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Scripting;

namespace CizaOptionModule
{
    public class OptionModule
    {
        private readonly PageModule _pageModule;
        private readonly Dictionary<int, Player> _playerMapByIndex = new Dictionary<int, Player>();

        private const int _minIndex = 0;
        private int _maxIndex;

        public const int NotInitialPageIndex = -1;

        public const int PercentRate = 100;

        // PlayerIndex
        public event Action<int> OnAddPlayer;

        // PlayerIndex
        public event Action<int> OnRemovePlayer;

        // PlayerIndex, PreviousOptionKey, CurrentOptionKey
        public event Action<int, string, string> OnSelect;

        // PlayerIndex, OptionKey, IsUnlock
        public event Action<int, string, bool> OnConfirm;

        public event Action<float> OnTick;

        // PreviousPageIndex, CurrentPageIndex
        public event Action<int, int> OnChangePage;

        public bool IsInitialized => _pageModule.IsInitialized;

        public int PlayerCount => _playerMapByIndex.Count;
        public int OptionDefaultPlayerIndex { get; private set; }

        public bool IsPageCircle { get; private set; }

        public bool IsAutoChangePage { get; private set; }

        public bool IsChangingPage { get; private set; }


        public int PageCount => _pageModule.GetAllPage<IOptionModulePage>().Length;
        public int CurrentPageIndex { get; private set; } = NotInitialPageIndex;

        public bool CanConfirm { get; private set; } = true;

        public float UnlockedOptionPercent
        {
            get
            {
                var allOptions = GetAllOptions<Option>();
                if (allOptions.Length <= 0)
                    return 0;

                var unlockedOptions = new HashSet<Option>();
                foreach (var option in allOptions)
                    if (option.IsUnlock)
                        unlockedOptions.Add(option);

                var unlockedOptionPercent = (float)unlockedOptions.Count / (float)allOptions.Length * PercentRate;
                return (float)Math.Round(Mathf.Clamp(unlockedOptionPercent, 0, PercentRate), 2);
            }
        }

        public bool CheckCanSelect(int playerIndex)
        {
            if (!IsInitialized || !_playerMapByIndex.TryGetValue(playerIndex, out var player))
                return false;

            return player.CanSelect;
        }

        public bool CheckCanConfirm(int playerIndex)
        {
            if (!IsInitialized || !_playerMapByIndex.TryGetValue(playerIndex, out var player))
                return false;

            return player.CanConfirm;
        }

        public bool TryGetCurrentCoordinate(int playerIndex, out Vector2Int currentCoordinate)
        {
            if (!IsInitialized || !_playerMapByIndex.TryGetValue(playerIndex, out var player) || !TryGetOptionModulePage(CurrentPageIndex, out var optionModulePage))
            {
                currentCoordinate = Vector2Int.zero;
                return false;
            }

            return optionModulePage.TryGetCurrentCoordinate(playerIndex, out currentCoordinate);
        }

        public bool TryGetCurrentOptionKey(int playerIndex, out string currentOptionKey)
        {
            if (!IsInitialized || !_playerMapByIndex.TryGetValue(playerIndex, out var player) || !TryGetOptionModulePage(CurrentPageIndex, out var optionModulePage))
            {
                currentOptionKey = string.Empty;
                return false;
            }

            return optionModulePage.TryGetCurrentOptionKey(playerIndex, out currentOptionKey);
        }

        public bool TryGetCurrentOption(int playerIndex, out Option currentOption)
        {
            if (!IsInitialized || !_playerMapByIndex.TryGetValue(playerIndex, out var player) || !TryGetOptionModulePage(CurrentPageIndex, out var optionModulePage) || !TryGetCurrentOptionKey(playerIndex, out var currentOptionKey))
            {
                currentOption = null;
                return false;
            }

            return optionModulePage.TryGetOption(currentOptionKey, out currentOption);
        }

        public bool TryGetOptionFromCurrentPage(string optionKey, out Option option)
        {
            if (!TryGetOptionModulePage(CurrentPageIndex, out var optionModulePage))
            {
                option = null;
                return false;
            }

            return optionModulePage.TryGetOption(optionKey, out option);
        }

        public bool TryGetSupOptionFromCurrentPage<TSupOption>(string optionKey, out TSupOption supOption) where TSupOption : class
        {
            if (!TryGetOptionModulePage(CurrentPageIndex, out var optionModulePage) || !optionModulePage.TryGetOption(optionKey, out var option))
            {
                supOption = null;
                return false;
            }

            supOption = option.GetComponent<TSupOption>();
            return supOption != null;
        }


        // PageIndex
        public TOption[][] GetAllOptionsList<TOption>() where TOption : class
        {
            var optionsList = new List<TOption[]>();
            for (var i = 0; i < PageCount; i++)
                if (TryGetOptionsFromPage<TOption>(i, out var options))
                    optionsList.Add(options);
            return optionsList.ToArray();
        }

        // PageIndex
        public TSupOption[][] GetAllSupOptionsList<TSupOption>() where TSupOption : class
        {
            var supOptionsList = new List<TSupOption[]>();
            for (var i = 0; i < PageCount; i++)
                if (TryGetSupOptionsFromPage<TSupOption>(i, out var supOptions))
                    supOptionsList.Add(supOptions);
            return supOptionsList.ToArray();
        }

        public TOption[] GetAllOptions<TOption>() where TOption : class
        {
            var optionList = new HashSet<TOption>();
            for (var i = 0; i < PageCount; i++)
                if (TryGetOptionsFromPage<TOption>(i, out var options))
                    foreach (var option in options)
                        optionList.Add(option);
            return optionList.ToArray();
        }

        public TSupOption[] GetAllSupOptions<TSupOption>() where TSupOption : class
        {
            var supOptionList = new HashSet<TSupOption>();
            for (var i = 0; i < PageCount; i++)
                if (TryGetSupOptionsFromPage<TSupOption>(i, out var supOptions))
                    foreach (var supOption in supOptions)
                        supOptionList.Add(supOption);
            return supOptionList.ToArray();
        }

        public bool TryGetOptionsFromPage<TOption>(int pageIndex, out TOption[] options) where TOption : class
        {
            if (_pageModule.TryGetPage<IOptionModulePage>(pageIndex.ToString(), out var optionModulePage))
            {
                var optionList = new HashSet<TOption>();
                foreach (var option in optionModulePage.GetAllOptions().Cast<TOption>().ToArray())
                    optionList.Add(option);

                options = optionList.ToArray();
                return true;
            }

            options = Array.Empty<TOption>();
            return false;
        }

        public bool TryGetSupOptionsFromPage<TSupOption>(int pageIndex, out TSupOption[] supOptions) where TSupOption : class
        {
            if (_pageModule.TryGetPage<IOptionModulePage>(pageIndex.ToString(), out var optionModulePage))
            {
                var supOptionList = new HashSet<TSupOption>();
                foreach (var option in optionModulePage.GetAllOptions())
                    if (option.TryGetComponent<TSupOption>(out var supOption))
                        supOptionList.Add(supOption);

                supOptions = supOptionList.ToArray();
                return true;
            }

            supOptions = Array.Empty<TSupOption>();
            return false;
        }

        [Preserve]
        public OptionModule(IOptionModuleConfig optionModuleConfig)
        {
            _pageModule = new PageModule(optionModuleConfig);
            _pageModule.OnTick += Tick;
        }

        public async UniTask InitializeAsync(int playerCount, Transform parent, IOptionModulePageInfo[] optionModulePageInfos, IOptionInfo[] optionInfos, bool isColumnCircle, int pageIndex = 0, Vector2Int coordinate = default, bool isAutoChangePage = false, int optionDefaultPlayerIndex = 0)
        {
            if (IsInitialized)
                return;

            OptionDefaultPlayerIndex = optionDefaultPlayerIndex;

            _maxIndex = optionModulePageInfos.Length - 1;
            IsPageCircle = isColumnCircle;
            IsAutoChangePage = isAutoChangePage;

            _pageModule.Initialize(parent);
            foreach (var optionModulePageInfo in optionModulePageInfos)
            {
                await _pageModule.CreateAsync<IOptionModulePage>(optionModulePageInfo.PageIndexString, this, OptionDefaultPlayerIndex, optionModulePageInfo, optionInfos);
                if (_pageModule.TryGetPage<IOptionModulePage>(optionModulePageInfo.PageIndexString, out var optionModulePage))
                {
                    optionModulePage.OnSelect += Select;
                    optionModulePage.OnConfirm += Confirm;
                }
            }

            if (!await TrySetPageIndexAsync(pageIndex, coordinate, false))
                await TrySetPageIndexAsync(0, coordinate, false);

            ResetPlayers(playerCount);
        }

        public void Release()
        {
            if (!IsInitialized)
                return;

            DisableAllCanSelect();
            DisableAllCanConfirm();

            ClearPlayers();
            _pageModule.Release();
            CurrentPageIndex = NotInitialPageIndex;
            IsChangingPage = false;
        }

        public void ResetPlayers(int playerCount)
        {
            ClearPlayers();
            for (var i = 0; i < playerCount; i++)
                AddPlayer(i, Vector2Int.zero);
        }

        public void ClearPlayers()
        {
            foreach (var playerIndex in _playerMapByIndex.Keys.ToArray())
                RemovePlayer(playerIndex);
        }

        public void AddPlayer(int playerIndex, Vector2Int coordinate, bool isEnableInput = true)
        {
            if (_playerMapByIndex.ContainsKey(playerIndex))
                return;

            _playerMapByIndex.Add(playerIndex, new Player(playerIndex));

            foreach (var optionModulePage in _pageModule.GetAllPage<IOptionModulePage>().ToArray())
                optionModulePage.AddPlayer(playerIndex);

            TrySetCurrentCoordinate(playerIndex, coordinate, true);

            if (isEnableInput)
                EnableInput(playerIndex);

            OnAddPlayer?.Invoke(playerIndex);
        }

        public void RemovePlayer(int playerIndex)
        {
            if (!_playerMapByIndex.ContainsKey(playerIndex))
                return;

            _playerMapByIndex.Remove(playerIndex);

            foreach (var optionModulePage in _pageModule.GetAllPage<IOptionModulePage>().ToArray())
                optionModulePage.RemovePlayer(playerIndex);

            OnRemovePlayer?.Invoke(playerIndex);
        }

        public UniTask ShowCurrentPageAsync(bool isImmediately = true, Func<UniTask> onCompleteBefore = null)
        {
            var player = _playerMapByIndex.Values.FirstOrDefault();
            if (player == null)
                return UniTask.CompletedTask;
            return ShowCurrentPageAsync(player.Index, isImmediately, onCompleteBefore);
        }

        public async UniTask ShowCurrentPageAsync(int playerIndex, bool isImmediately = true, Func<UniTask> onCompleteBefore = null)
        {
            if (!IsInitialized || CurrentPageIndex == NotInitialPageIndex || _pageModule.CheckIsShowing(CurrentPageIndex.ToString()) || _pageModule.CheckIsHiding(CurrentPageIndex.ToString()))
                return;

            if (!TryGetCurrentCoordinate(playerIndex, out var currentCoordinate))
                return;

            if (_pageModule.CheckIsVisible(CurrentPageIndex.ToString()))
                _pageModule.HideImmediately(CurrentPageIndex.ToString());

            if (isImmediately)
                await _pageModule.ShowImmediatelyAsync(CurrentPageIndex.ToString(), null, false, currentCoordinate, false);
            else
                await _pageModule.ShowAsync(CurrentPageIndex.ToString(), null, false, currentCoordinate, false);

            if (onCompleteBefore != null)
                await onCompleteBefore.Invoke();

            _pageModule.OnlyCallShowingComplete(CurrentPageIndex.ToString());
        }

        public async UniTask HideCurrentPageAsync(bool isImmediately = true, Func<UniTask> onCompleteBefore = null)
        {
            if (!IsInitialized || CurrentPageIndex == NotInitialPageIndex || !_pageModule.CheckIsVisible(CurrentPageIndex.ToString()))
                return;

            if (isImmediately)
                _pageModule.HideImmediately(CurrentPageIndex.ToString(), isIncludeHidingComplete: false);
            else
                await _pageModule.HideAsync(CurrentPageIndex.ToString(), isIncludeHidingComplete: false);

            if (onCompleteBefore != null)
                await onCompleteBefore.Invoke();

            _pageModule.OnlyCallHidingComplete(CurrentPageIndex.ToString());
        }

        public void SetCanConfirm(bool canConfirm) =>
            CanConfirm = canConfirm;

        public bool TryConfirm(int playerIndex)
        {
            if (!IsInitialized || !TryGetOptionModulePage(CurrentPageIndex, out var optionModulePage) || !_playerMapByIndex.TryGetValue(playerIndex, out var player) || !player.CanConfirm || !CanConfirm)
                return false;

            return optionModulePage.TryConfirm(playerIndex);
        }

        public async UniTask<bool> TrySetPageIndexAsync(int pageIndex, Vector2Int coordinate, bool isAutoTurnOffIsNew = true, bool isHideImmediately = true, bool isShowImmediately = true)
        {
            if (!IsInitialized || (pageIndex < _minIndex && pageIndex > _maxIndex))
                return false;

            IsChangingPage = true;

            var previousPageIndex = CurrentPageIndex;
            CurrentPageIndex = pageIndex;

            if (previousPageIndex != NotInitialPageIndex && _pageModule.CheckIsVisible(previousPageIndex.ToString()))
            {
                if (isHideImmediately)
                    _pageModule.HideImmediately(previousPageIndex.ToString(), isIncludeHidingComplete: false);
                else
                    await _pageModule.HideAsync(previousPageIndex.ToString(), isIncludeHidingComplete: false);
            }

            if (previousPageIndex != CurrentPageIndex)
                await _pageModule.OnlyCallShowingPrepareAsync(CurrentPageIndex.ToString(), null, coordinate, isAutoTurnOffIsNew);

            if (previousPageIndex != NotInitialPageIndex && _pageModule.CheckIsHiding(previousPageIndex.ToString()))
                _pageModule.OnlyCallHidingComplete(previousPageIndex.ToString());

            if (isShowImmediately)
                await _pageModule.ShowImmediatelyAsync(CurrentPageIndex.ToString(), null, true, coordinate, isAutoTurnOffIsNew);
            else
                await _pageModule.ShowAsync(CurrentPageIndex.ToString(), null, true, coordinate, isAutoTurnOffIsNew);

            OnChangePage?.Invoke(previousPageIndex, CurrentPageIndex);

            IsChangingPage = false;

            return true;
        }

        public bool TrySetCurrentCoordinate(int playerIndex, Vector2Int coordinate, bool isIgnoreCanSelect)
        {
            if (!IsInitialized || !_playerMapByIndex.TryGetValue(playerIndex, out var player) || (!isIgnoreCanSelect && !player.CanSelect))
                return false;

            if (!TryGetOptionModulePage(CurrentPageIndex, out var optionModulePage))
                return false;

            return optionModulePage.TrySetCurrentCoordinate(playerIndex, coordinate);
        }

        public bool TrySetCurrentCoordinate(int playerIndex, string optionKey, bool isIgnoreCanSelect)
        {
            if (!IsInitialized || !_playerMapByIndex.TryGetValue(playerIndex, out var player) || (!isIgnoreCanSelect && !player.CanSelect))
                return false;

            if (!TryGetOptionModulePage(CurrentPageIndex, out var optionModulePage))
                return false;

            return optionModulePage.TrySetCurrentCoordinate(playerIndex, optionKey, false);
        }

        public async UniTask<bool> TryMovePageToLeftAsync(int playerIndex, bool isImmediately = true)
        {
            if (!TryGetCurrentCoordinate(playerIndex, out var currentCoordinate))
                return false;

            var nextPageIndex = (CurrentPageIndex - 1).ToClamp(_minIndex, _maxIndex, IsPageCircle);
            if (nextPageIndex == CurrentPageIndex)
                return false;

            if (!TryGetOptionModulePage(nextPageIndex, out var nextOptionModulePage))
                return false;

            var coordinate = IsAutoChangePage ? new Vector2Int(nextOptionModulePage.MaxColumnIndex, currentCoordinate.y.ToClamp(0, nextOptionModulePage.MaxRowIndex)) : currentCoordinate;
            return await TrySetPageIndexAsync(nextPageIndex, coordinate, isHideImmediately: isImmediately, isShowImmediately: isImmediately);
        }

        public async UniTask<bool> TryMovePageToRightAsync(int playerIndex, bool isImmediately = true)
        {
            if (!TryGetCurrentCoordinate(playerIndex, out var currentCoordinate))
                return false;

            var nextPageIndex = (CurrentPageIndex + 1).ToClamp(_minIndex, _maxIndex, IsPageCircle);
            if (nextPageIndex == CurrentPageIndex)
                return false;

            if (!TryGetOptionModulePage(nextPageIndex, out var nextOptionModulePage))
                return false;

            var coordinate = IsAutoChangePage ? new Vector2Int(0, currentCoordinate.y.ToClamp(0, nextOptionModulePage.MaxRowIndex)) : currentCoordinate;
            return await TrySetPageIndexAsync(nextPageIndex, coordinate, isHideImmediately: isImmediately, isShowImmediately: isImmediately);
        }

        public void EnableAllInput()
        {
            EnableAllCanSelect();
            EnableAllCanConfirm();
        }

        public void DisableAllInput()
        {
            DisableAllCanSelect();
            DisableAllCanConfirm();
        }


        public void EnableInput(int playerIndex)
        {
            EnableCanSelect(playerIndex);
            EnableCanConfirm(playerIndex);
        }

        public void DisableInput(int playerIndex)
        {
            DisableCanSelect(playerIndex);
            DisableCanConfirm(playerIndex);
        }

        public void EnableAllCanSelect()
        {
            foreach (var playerIndex in _playerMapByIndex.Keys.ToArray())
                EnableCanSelect(playerIndex);
        }

        public void DisableAllCanSelect()
        {
            foreach (var playerIndex in _playerMapByIndex.Keys.ToArray())
                DisableCanSelect(playerIndex);
        }

        public void EnableCanSelect(int playerIndex)
        {
            if (!_playerMapByIndex.TryGetValue(playerIndex, out var player))
                return;

            player.EnableCanSelect();
        }

        public void DisableCanSelect(int playerIndex)
        {
            if (!_playerMapByIndex.TryGetValue(playerIndex, out var player))
                return;

            player.DisableCanSelect();
        }

        public void EnableAllCanConfirm()
        {
            foreach (var playerIndex in _playerMapByIndex.Keys.ToArray())
                EnableCanConfirm(playerIndex);
        }

        public void DisableAllCanConfirm()
        {
            foreach (var playerIndex in _playerMapByIndex.Keys.ToArray())
                DisableCanConfirm(playerIndex);
        }

        public void EnableCanConfirm(int playerIndex)
        {
            if (!_playerMapByIndex.TryGetValue(playerIndex, out var player))
                return;

            player.EnableCanConfirm();
        }

        public void DisableCanConfirm(int playerIndex)
        {
            if (!_playerMapByIndex.TryGetValue(playerIndex, out var player))
                return;

            player.DisableCanConfirm();
        }

        public async UniTask<bool> TryMoveOptionToLeftAsync(int playerIndex, bool isImmediately = true)
        {
            if (!IsInitialized || !_playerMapByIndex.TryGetValue(playerIndex, out var player) || !player.CanSelect || !TryGetOptionModulePage(CurrentPageIndex, out var optionModulePage))
                return false;

            if (!optionModulePage.TryMoveToLeft(playerIndex))
                return IsAutoChangePage && await TryMovePageToLeftAsync(playerIndex, isImmediately);

            return true;
        }

        public async UniTask<bool> TryMoveOptionToRightAsync(int playerIndex, bool isImmediately = true)
        {
            if (!IsInitialized || !_playerMapByIndex.TryGetValue(playerIndex, out var player) || !player.CanSelect || !TryGetOptionModulePage(CurrentPageIndex, out var optionModulePage))
                return false;

            if (!optionModulePage.TryMoveToRight(playerIndex))
                return IsAutoChangePage && await TryMovePageToRightAsync(playerIndex, isImmediately);

            return true;
        }

        public bool TryMoveOptionToUp(int playerIndex)
        {
            if (!IsInitialized || !_playerMapByIndex.TryGetValue(playerIndex, out var player) || !player.CanSelect || !TryGetOptionModulePage(CurrentPageIndex, out var optionModulePage))
                return false;

            return optionModulePage.TryMoveToUp(playerIndex);
        }

        public bool TryMoveOptionToDown(int playerIndex)
        {
            if (!IsInitialized || !_playerMapByIndex.TryGetValue(playerIndex, out var player) || !player.CanSelect || !TryGetOptionModulePage(CurrentPageIndex, out var optionModulePage))
                return false;

            return optionModulePage.TryMoveToDown(playerIndex);
        }

        internal bool TrySetCurrentCoordinateFromPointerEnter(int playerIndex, string optionKey, bool isIgnoreCanSelect)
        {
            if (!IsInitialized || !_playerMapByIndex.TryGetValue(playerIndex, out var player) || (!isIgnoreCanSelect && !player.CanSelect))
                return false;

            if (!TryGetOptionModulePage(CurrentPageIndex, out var optionModulePage))
                return false;

            return optionModulePage.TrySetCurrentCoordinate(playerIndex, optionKey, true);
        }

        private bool TryGetOptionModulePage(int pageIndex, out IOptionModulePage optionModulePage)
        {
            if (!IsInitialized)
            {
                optionModulePage = null;
                return false;
            }

            if (!_pageModule.TryGetPage(pageIndex.ToString(), out optionModulePage))
                return false;

            return true;
        }

        private void Select(int playerIndex, string previousOptionKey, string currentOptionKey) =>
            OnSelect?.Invoke(playerIndex, previousOptionKey, currentOptionKey);

        private void Confirm(int playerIndex, string optionKey, bool isUnlock) =>
            OnConfirm?.Invoke(playerIndex, optionKey, isUnlock);

        private void Tick(float deltaTime) =>
            OnTick?.Invoke(deltaTime);

        private class Player
        {
            public int Index { get; }

            public bool CanSelect { get; private set; }

            public bool CanConfirm { get; private set; }

            public Player(int index) =>
                Index = index;

            public void EnableCanSelect() =>
                CanSelect = true;

            public void DisableCanSelect() =>
                CanSelect = false;

            public void EnableCanConfirm() =>
                CanConfirm = true;

            public void DisableCanConfirm() =>
                CanConfirm = false;
        }
    }
}