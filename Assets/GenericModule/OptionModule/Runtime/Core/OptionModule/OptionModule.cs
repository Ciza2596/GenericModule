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

		public event Action<string, string> OnSelect;
		public event Action<string, bool>   OnConfirm;

		public bool IsInitialized => _pageModule.IsInitialized;

		public bool IsPageCircle { get; private set; }

		public bool IsAutoChangePage { get; private set; }

		public bool IsChangingPage { get; private set; }

		public int CurrentPageIndex { get; private set; } = NotInitialPageIndex;

		public Vector2Int CurrentCoordinate
		{
			get
			{
				if (!TryGetOptionModulePage(CurrentPageIndex, out var optionModulePage))
					return Vector2Int.zero;

				return optionModulePage.CurrentCoordinate;
			}
		}

		public string OptionKey
		{
			get
			{
				if (!TryGetOptionModulePage(CurrentPageIndex, out var optionModulePage))
					return string.Empty;

				return optionModulePage.OptionKey;
			}
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

		public async UniTask InitializeAsync(Transform optionModulePageRootParentTransform, IOptionModulePageInfo[] optionModulePageInfos, IOptionInfo[] optionInfos, bool isColumnCircle, int pageIndex = 0, Vector2Int coordinate = default, bool isAutoChangePage = false, params object[] parameters)
		{
			if (IsInitialized)
				return;

			_maxIndex        = optionModulePageInfos.Length - 1;
			IsPageCircle     = isColumnCircle;
			IsAutoChangePage = isAutoChangePage;

			_pageModule.Initialize(optionModulePageRootParentTransform);
			foreach (var optionModulePageInfo in optionModulePageInfos)
			{
				await _pageModule.CreateAsync<IOptionModulePage>(optionModulePageInfo.PageIndexString, optionModulePageInfo, optionInfos, parameters);
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

		public bool TryConfirm()
		{
			if (!TryGetOptionModulePage(CurrentPageIndex, out var optionModulePage))
				return false;

			return optionModulePage.TryConfirm();
		}

		public async UniTask<bool> TrySetPageIndexAsync(int index, Vector2Int coordinate, bool isAutoTurnOffIsNew = true)
		{
			if (!IsInitialized)
				return false;

			if (index < _minIndex && index > _maxIndex)
				return false;

			IsChangingPage = true;

			if (CurrentPageIndex != NotInitialPageIndex)
				await _pageModule.HideAsync(CurrentPageIndex.ToString());

			CurrentPageIndex = index;
			await _pageModule.ShowAsync(CurrentPageIndex.ToString(), null, true, coordinate, isAutoTurnOffIsNew);

			IsChangingPage = false;

			return true;
		}

		public bool TrySetCoordinateAsync(Vector2Int coordinate)
		{
			if (!IsInitialized)
				return false;

			if (!TryGetOptionModulePage(CurrentPageIndex, out var optionModulePage))
				return false;

			return optionModulePage.TrySetCurrentCoordinate(coordinate);
		}

		public async UniTask<bool> TryMovePageToLeftAsync()
		{
			var nextPageIndex = (CurrentPageIndex - 1).ToClamp(_minIndex, _maxIndex, IsPageCircle);
			if (nextPageIndex == CurrentPageIndex)
				return false;

			if (!TryGetOptionModulePage(nextPageIndex, out var nextOptionModulePage))
				return false;

			var coordinate = IsAutoChangePage ? new Vector2Int(nextOptionModulePage.MaxColumnIndex, CurrentCoordinate.y.ToClamp(0, nextOptionModulePage.MaxRowIndex)) : CurrentCoordinate;
			return await TrySetPageIndexAsync(nextPageIndex, coordinate);
		}

		public async UniTask<bool> TryMovePageToRightAsync()
		{
			var nextPageIndex = (CurrentPageIndex + 1).ToClamp(_minIndex, _maxIndex, IsPageCircle);
			if (nextPageIndex == CurrentPageIndex)
				return false;

			if (!TryGetOptionModulePage(nextPageIndex, out var nextOptionModulePage))
				return false;

			var coordinate = IsAutoChangePage ? new Vector2Int(0, CurrentCoordinate.y.ToClamp(0, nextOptionModulePage.MaxRowIndex)) : CurrentCoordinate;
			return await TrySetPageIndexAsync(nextPageIndex, coordinate);
		}

		public async UniTask<bool> TryMoveOptionToLeftAsync()
		{
			if (!TryGetOptionModulePage(CurrentPageIndex, out var optionModulePage))
				return false;

			if (!optionModulePage.TryMoveToLeft())
				return IsAutoChangePage && await TryMovePageToLeftAsync();

			return true;
		}

		public async UniTask<bool> TryMoveOptionToRightAsync()
		{
			if (!TryGetOptionModulePage(CurrentPageIndex, out var optionModulePage))
				return false;

			if (!optionModulePage.TryMoveToRight())
				return IsAutoChangePage && await TryMovePageToRightAsync();

			return true;
		}

		public bool TryMoveOptionToUp()
		{
			if (!TryGetOptionModulePage(CurrentPageIndex, out var optionModulePage))
				return false;

			return optionModulePage.TryMoveToUp();
		}

		public bool TryMoveOptionToDown()
		{
			if (!TryGetOptionModulePage(CurrentPageIndex, out var optionModulePage))
				return false;

			return optionModulePage.TryMoveToDown();
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

		private void Select(string previousOptionKey, string currentOptionKey) =>
			OnSelect?.Invoke(previousOptionKey, currentOptionKey);

		private void Confirm(string optionKey, bool isUnlock) =>
			OnConfirm?.Invoke(optionKey, isUnlock);
	}
}
