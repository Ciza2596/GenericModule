using System;
using System.Collections.Generic;
using CizaCore;
using CizaPageModule;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CizaOptionModule
{
	public class OptionModule
	{
		private readonly PageModule _pageModule;

		private readonly int _minIndex = 0;
		private          int _maxIndex;

		public const int NotInitialPageIndex = -1;

		// PlayerIndex, PreviousCoordinate, PreviousOption, CurrentCoordinate, CurrentOption
		public event Action<int, string, string> OnSelect;

		// PlayerIndex, OptionKey, IsUnlock
		public event Action<int, string, bool> OnConfirm;

		public bool IsInitialized => _pageModule.IsInitialized;

		public int PlayerCount              { get; private set; }
		public int OptionDefaultPlayerIndex { get; private set; }

		public bool IsPageCircle { get; private set; }

		public bool IsAutoChangePage { get; private set; }

		public bool IsChangingPage { get; private set; }

		public int CurrentPageIndex { get; private set; } = NotInitialPageIndex;

		public bool TryGetCurrentCoordinate(int playerIndex, out Vector2Int currentCoordinate)
		{
			if (!ValidatePlayerIndex(playerIndex) || !TryGetOptionModulePage(CurrentPageIndex, out var optionModulePage))
			{
				currentCoordinate = Vector2Int.zero;
				return false;
			}

			return optionModulePage.TryGetCurrentCoordinate(playerIndex, out currentCoordinate);
		}

		public bool TryGetCurrentOptionKey(int playerIndex, out string currentOptionKey)
		{
			if (!ValidatePlayerIndex(playerIndex) || !TryGetOptionModulePage(CurrentPageIndex, out var optionModulePage))
			{
				currentOptionKey = string.Empty;
				return false;
			}

			return optionModulePage.TryGetCurrentOptionKey(playerIndex, out currentOptionKey);
		}
		
		public bool TryGetCurrentOption(int playerIndex, out Option currentOption)
		{
			if (!ValidatePlayerIndex(playerIndex) || !TryGetOptionModulePage(CurrentPageIndex, out var optionModulePage) || !TryGetCurrentOptionKey(playerIndex, out var currentOptionKey))
			{
				currentOption = null;
				return false;
			}

			return optionModulePage.TryGetOption(currentOptionKey, out currentOption);
		}

		public bool TryGetOptionFromCurrentPage(string key, out Option option)
		{
			if (!TryGetOptionModulePage(CurrentPageIndex, out var optionModulePage))
			{
				option = null;
				return false;
			}

			return optionModulePage.TryGetOption(key, out option);
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

			PlayerCount              = playerCount;
			OptionDefaultPlayerIndex = optionDefaultPlayerIndex;

			_maxIndex        = optionModulePageInfos.Length - 1;
			IsPageCircle     = isColumnCircle;
			IsAutoChangePage = isAutoChangePage;

			_pageModule.Initialize(parent);
			foreach (var optionModulePageInfo in optionModulePageInfos)
			{
				await _pageModule.CreateAsync<IOptionModulePage>(optionModulePageInfo.PageIndexString, PlayerCount, OptionDefaultPlayerIndex, optionModulePageInfo, optionInfos, parameters);
				if (_pageModule.TryGetPage<IOptionModulePage>(optionModulePageInfo.PageIndexString, out var optionModulePage))
				{
					optionModulePage.OnSelect  += Select;
					optionModulePage.OnConfirm += Confirm;
				}
			}

			if (!await TrySetPageIndexAsync(pageIndex, coordinate, false))
				await TrySetPageIndexAsync(0, coordinate, false);
		}

		public void Release()
		{
			if (!IsInitialized)
				return;

			_pageModule.Release();
			CurrentPageIndex = NotInitialPageIndex;
			IsChangingPage   = false;
		}

		public bool TryConfirm(int playerIndex)
		{
			if (!IsInitialized || !ValidatePlayerIndex(playerIndex) || !TryGetOptionModulePage(CurrentPageIndex, out var optionModulePage))
				return false;

			return optionModulePage.TryConfirm(playerIndex);
		}

		public async UniTask<bool> TrySetPageIndexAsync(int pageIndex, Vector2Int coordinate, bool isAutoTurnOffIsNew = true)
		{
			if (!IsInitialized)
				return false;

			if (pageIndex < _minIndex && pageIndex > _maxIndex)
				return false;

			IsChangingPage = true;

			if (CurrentPageIndex != NotInitialPageIndex)
				await _pageModule.HideAsync(CurrentPageIndex.ToString());

			CurrentPageIndex = pageIndex;
			await _pageModule.ShowAsync(CurrentPageIndex.ToString(), null, true, coordinate, isAutoTurnOffIsNew);

			IsChangingPage = false;

			return true;
		}

		public bool TrySetCoordinateAsync(int playerIndex, Vector2Int coordinate)
		{
			if (!IsInitialized || !ValidatePlayerIndex(playerIndex))
				return false;

			if (!TryGetOptionModulePage(CurrentPageIndex, out var optionModulePage))
				return false;

			return optionModulePage.TrySetCurrentCoordinate(playerIndex, coordinate);
		}

		public async UniTask<bool> TryMovePageToLeftAsync(int playerIndex)
		{
			if (!TryGetCurrentCoordinate(playerIndex, out var currentCoordinate))
				return false;

			var nextPageIndex = (CurrentPageIndex - 1).ToClamp(_minIndex, _maxIndex, IsPageCircle);
			if (nextPageIndex == CurrentPageIndex)
				return false;

			if (!TryGetOptionModulePage(nextPageIndex, out var nextOptionModulePage))
				return false;

			var coordinate = IsAutoChangePage ? new Vector2Int(nextOptionModulePage.MaxColumnIndex, currentCoordinate.y.ToClamp(0, nextOptionModulePage.MaxRowIndex)) : currentCoordinate;
			return await TrySetPageIndexAsync(nextPageIndex, coordinate);
		}

		public async UniTask<bool> TryMovePageToRightAsync(int playerIndex)
		{
			if (!TryGetCurrentCoordinate(playerIndex, out var currentCoordinate))
				return false;

			var nextPageIndex = (CurrentPageIndex + 1).ToClamp(_minIndex, _maxIndex, IsPageCircle);
			if (nextPageIndex == CurrentPageIndex)
				return false;

			if (!TryGetOptionModulePage(nextPageIndex, out var nextOptionModulePage))
				return false;

			var coordinate = IsAutoChangePage ? new Vector2Int(0, currentCoordinate.y.ToClamp(0, nextOptionModulePage.MaxRowIndex)) : currentCoordinate;
			return await TrySetPageIndexAsync(nextPageIndex, coordinate);
		}

		public async UniTask<bool> TryMoveOptionToLeftAsync(int playerIndex)
		{
			if (!ValidatePlayerIndex(playerIndex) || !TryGetOptionModulePage(CurrentPageIndex, out var optionModulePage))
				return false;

			if (!optionModulePage.TryMoveToLeft(playerIndex))
				return IsAutoChangePage && await TryMovePageToLeftAsync(playerIndex);

			return true;
		}

		public async UniTask<bool> TryMoveOptionToRightAsync(int playerIndex)
		{
			if (!ValidatePlayerIndex(playerIndex) || !TryGetOptionModulePage(CurrentPageIndex, out var optionModulePage))
				return false;

			if (!optionModulePage.TryMoveToRight(playerIndex))
				return IsAutoChangePage && await TryMovePageToRightAsync(playerIndex);

			return true;
		}

		public bool TryMoveOptionToUp(int playerIndex)
		{
			if (!TryGetOptionModulePage(CurrentPageIndex, out var optionModulePage))
				return false;

			return optionModulePage.TryMoveToUp(playerIndex);
		}

		public bool TryMoveOptionToDown(int playerIndex)
		{
			if (!TryGetOptionModulePage(CurrentPageIndex, out var optionModulePage))
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

		private bool ValidatePlayerIndex(int playerIndex) =>
			playerIndex < PlayerCount;
	}
}
