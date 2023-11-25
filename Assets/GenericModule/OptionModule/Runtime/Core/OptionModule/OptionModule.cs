using System;
using System.Collections.Generic;
using System.Linq;
using CizaCore;
using CizaPageModule;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CizaOptionModule
{
	public class OptionModule
	{
		private readonly PageModule              _pageModule;
		private readonly Dictionary<int, Player> _playerMapByIndex = new Dictionary<int, Player>();

		private const int _minIndex = 0;
		private       int _maxIndex;

		public const int NotInitialPageIndex = -1;

		// PlayerIndex, PreviousCoordinate, PreviousOption, CurrentCoordinate, CurrentOption
		public event Action<int, string, string> OnSelect;

		// PlayerIndex, OptionKey, IsUnlock
		public event Action<int, string, bool> OnConfirm;

		public bool IsInitialized => _pageModule.IsInitialized;

		public int PlayerCount              => _playerMapByIndex.Count;
		public int OptionDefaultPlayerIndex { get; private set; }

		public bool IsPageCircle { get; private set; }

		public bool IsAutoChangePage { get; private set; }

		public bool IsChangingPage { get; private set; }

		public int CurrentPageIndex { get; private set; } = NotInitialPageIndex;

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

		public TOption[] GetAllOptions<TOption>() where TOption : class
		{
			var options = new List<TOption>();
			foreach (var optionModulePage in _pageModule.GetAllPage<IOptionModulePage>())
				foreach (var option in optionModulePage.GetAllOptions())
					if (option is TOption tOption)
						options.Add(tOption);

			return options.ToArray();
		}

		public TSubOption[] GetAllSubOptions<TSubOption>() where TSubOption : class
		{
			var subOptions = new List<TSubOption>();
			foreach (var option in GetAllOptions<Option>())
				if (option.TryGetComponent<TSubOption>(out var subOption))
					subOptions.Add(subOption);

			return subOptions.ToArray();
		}

		public OptionModule(IOptionModuleConfig optionModuleConfig) =>
			_pageModule = new PageModule(optionModuleConfig);

		public async UniTask InitializeAsync(int playerCount, Transform parent, IOptionModulePageInfo[] optionModulePageInfos, IOptionInfo[] optionInfos, bool isColumnCircle, int pageIndex = 0, Vector2Int coordinate = default, bool isAutoChangePage = false, int optionDefaultPlayerIndex = 0, params object[] parameters)
		{
			if (IsInitialized)
				return;

			CreatePlayers(playerCount);
			OptionDefaultPlayerIndex = optionDefaultPlayerIndex;

			_maxIndex        = optionModulePageInfos.Length - 1;
			IsPageCircle     = isColumnCircle;
			IsAutoChangePage = isAutoChangePage;

			_pageModule.Initialize(parent);
			foreach (var optionModulePageInfo in optionModulePageInfos)
			{
				await _pageModule.CreateAsync<IOptionModulePage>(optionModulePageInfo.PageIndexString, this, PlayerCount, OptionDefaultPlayerIndex, optionModulePageInfo, optionInfos, parameters);
				if (_pageModule.TryGetPage<IOptionModulePage>(optionModulePageInfo.PageIndexString, out var optionModulePage))
				{
					optionModulePage.OnSelect  += Select;
					optionModulePage.OnConfirm += Confirm;
				}
			}

			if (!await TrySetPageIndexAsync(pageIndex, coordinate, false))
				await TrySetPageIndexAsync(0, coordinate, false);

			EnableAllCanSelect();
			EnableAllCanConfirm();
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
			IsChangingPage   = false;
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
				await (UniTask)onCompleteBefore?.Invoke();
			
			_pageModule.OnlyCallHidingComplete(CurrentPageIndex.ToString());
		}

		public bool TryConfirm(int playerIndex)
		{
			if (!IsInitialized || !TryGetOptionModulePage(CurrentPageIndex, out var optionModulePage) || !_playerMapByIndex.TryGetValue(playerIndex, out var player) || !player.CanConfirm)
				return false;

			return optionModulePage.TryConfirm(playerIndex);
		}

		public async UniTask<bool> TrySetPageIndexAsync(int pageIndex, Vector2Int coordinate, bool isAutoTurnOffIsNew = true, bool isImmediately = true)
		{
			if (!IsInitialized || (pageIndex < _minIndex && pageIndex > _maxIndex))
				return false;

			IsChangingPage = true;

			var previousCurrentPageIndex = CurrentPageIndex;
			CurrentPageIndex = pageIndex;

			if (previousCurrentPageIndex != NotInitialPageIndex && _pageModule.CheckIsVisible(previousCurrentPageIndex.ToString()))
			{
				if (isImmediately)
					_pageModule.HideImmediately(previousCurrentPageIndex.ToString(), isIncludeHidingComplete: false);
				else
					await _pageModule.HideAsync(previousCurrentPageIndex.ToString(), isIncludeHidingComplete: false);
			}

			await _pageModule.OnlyCallShowingPrepareAsync(CurrentPageIndex.ToString(), null, true, coordinate, isAutoTurnOffIsNew);

			if (_pageModule.CheckIsHiding(previousCurrentPageIndex.ToString()))
				_pageModule.OnlyCallHidingComplete(previousCurrentPageIndex.ToString());

			if (isImmediately)
				await _pageModule.ShowImmediatelyAsync(CurrentPageIndex.ToString(), null, true, coordinate, isAutoTurnOffIsNew);

			else
				await _pageModule.ShowAsync(CurrentPageIndex.ToString(), null, true, coordinate, isAutoTurnOffIsNew);

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

			return optionModulePage.TrySetCurrentCoordinate(playerIndex, optionKey);
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
			return await TrySetPageIndexAsync(nextPageIndex, coordinate, isImmediately: isImmediately);
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
			return await TrySetPageIndexAsync(nextPageIndex, coordinate, isImmediately: isImmediately);
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

		public async UniTask<bool> TryMoveOptionToLeftAsync(int playerIndex)
		{
			if (!IsInitialized || !_playerMapByIndex.TryGetValue(playerIndex, out var player) || !player.CanSelect || !TryGetOptionModulePage(CurrentPageIndex, out var optionModulePage))
				return false;

			if (!optionModulePage.TryMoveToLeft(playerIndex))
				return IsAutoChangePage && await TryMovePageToLeftAsync(playerIndex);

			return true;
		}

		public async UniTask<bool> TryMoveOptionToRightAsync(int playerIndex)
		{
			if (!IsInitialized || !_playerMapByIndex.TryGetValue(playerIndex, out var player) || !player.CanSelect || !TryGetOptionModulePage(CurrentPageIndex, out var optionModulePage))
				return false;

			if (!optionModulePage.TryMoveToRight(playerIndex))
				return IsAutoChangePage && await TryMovePageToRightAsync(playerIndex);

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

		private void CreatePlayers(int playerCount)
		{
			for (var i = 0; i < playerCount; i++)
				_playerMapByIndex.Add(i, new Player(i));
		}

		private void ClearPlayers() =>
			_playerMapByIndex.Clear();

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
