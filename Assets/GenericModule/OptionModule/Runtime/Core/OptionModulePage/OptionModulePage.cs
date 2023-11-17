using System;
using System.Linq;
using CizaPageModule;
using CizaPageModule.Implement;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

namespace CizaOptionModule
{
	public abstract class OptionModulePage : Page, IOptionModulePage, IInitializable, IShowingPrepare, IShowingStart, IHidingStart, IReleasable
	{
		[SerializeField]
		protected Transform _parentTransform;

		protected readonly SelectOptionLogic _selectOptionLogic = new SelectOptionLogic();

		protected IOptionView _optionView;
		protected Vector2Int  _onShowingStartCoordinate;

		protected bool _isAutoTurnOffIsNew;
		protected bool _localIsAutoTurnOffIsNew;

		public event Action<int, string, string> OnSelect;
		public event Action<int, string, bool>   OnConfirm;

		public int PageIndex { get; private set; }

		public int MaxColumnIndex => _selectOptionLogic.MaxColumnLength - 1;
		public int MaxRowIndex    => _selectOptionLogic.MaxRowLength    - 1;

		public virtual UniTask InitializeAsync(params object[] parameters)
		{
			var playerCount              = (int)parameters[0];
			var optionDefaultPlayerIndex = (int)parameters[1];

			var optionModulePageInfo = parameters[2] as IOptionModulePageInfo;
			Assert.IsNotNull(optionModulePageInfo, $"[{GetType().Name}::Initialize] OptionModulePageInfo is not found.");

			PageIndex = int.Parse(optionModulePageInfo.PageIndexString);

			var optionViewGameObject = Instantiate(optionModulePageInfo.OptionViewPrefab, _parentTransform);
			_optionView = optionViewGameObject.GetComponent<IOptionView>();

			var optionInfos = parameters[3] as IOptionInfo[];
			_optionView.OptionsIncludeNull.InitializeOptions(optionDefaultPlayerIndex, optionModulePageInfo.OptionKeys, optionInfos, OnConfirmImp, OnPointerEnter, GetType().Name);

			_selectOptionLogic.Initialize(playerCount, _optionView.OptionColumns, _optionView.Options, _optionView.ColumnInfo, _optionView.RowInfo);
			_selectOptionLogic.OnSetCurrentCoordinate += OnSetCurrentCoordinate;

			return UniTask.CompletedTask;
		}

		public UniTask OnShowingPrepareAsync(params object[] parameters)
		{
			_onShowingStartCoordinate = (Vector2Int)parameters[0];
			_isAutoTurnOffIsNew       = (bool)parameters[1];
			return UniTask.CompletedTask;
		}

		public void OnShowingStart()
		{
			_optionView.UnSelectAll();
			_localIsAutoTurnOffIsNew = true;

			for (var i = 0; i < _selectOptionLogic.PlayerCount; i++)
				_selectOptionLogic.TrySetCurrentCoordinate(i, _onShowingStartCoordinate);
		}

		public virtual void OnHidingStart() =>
			_localIsAutoTurnOffIsNew = false;

		public void Release() =>
			_selectOptionLogic.OnSetCurrentCoordinate += OnSetCurrentCoordinate;

		public Option[] GetAllOptions() =>
			_optionView.Options.ToArray();

		public bool TryGetOption(string optionKey, out Option option)
		{
			option = _optionView.Options.FirstOrDefault(m_option => m_option.Key == optionKey);
			return option != null;
		}

		public bool TryGetCurrentCoordinate(int playerIndex, out Vector2Int currentCoordinate) =>
			_selectOptionLogic.TryGetCurrentCoordinate(playerIndex, out currentCoordinate);

		public bool TryGetCurrentOptionKey(int playerIndex, out string currentOptionKey) =>
			_selectOptionLogic.TryGetCurrentOptionKey(playerIndex, out currentOptionKey);

		public bool TrySetCurrentCoordinate(int playerIndex, Vector2Int coordinate) =>
			_selectOptionLogic.TrySetCurrentCoordinate(playerIndex, coordinate);

		public bool TryConfirm(int playerIndex)
		{
			if (!_selectOptionLogic.TryGetCurrentCoordinate(playerIndex, out var currentCoordinate))
				return false;

			if (!_selectOptionLogic.TryGetOption(currentCoordinate, out var option))
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

		protected virtual void OnPointerEnter(int playerIndex, string optionKey) =>
			_selectOptionLogic.TrySetCurrentCoordinate(playerIndex, optionKey);

		protected virtual void OnSetCurrentCoordinate(int playerIndex, Vector2Int previousCoordinate, Option previousOption, Vector2Int currentCoordinate, Option currentOption)
		{
			if (!CheckIsAnyPlayerOnCoordinate(previousCoordinate))
				previousOption?.Unselect();

			currentOption?.Select(_isAutoTurnOffIsNew && _localIsAutoTurnOffIsNew);
			OnSelect?.Invoke(playerIndex, previousOption != null ? previousOption.Key : string.Empty, currentOption != null ? currentOption.Key : string.Empty);
		}

		protected bool CheckIsAnyPlayerOnCoordinate(Vector2Int coordinate)
		{
			for (var i = 0; i < _selectOptionLogic.PlayerCount; i++)
				if (TryGetCurrentCoordinate(i, out var currentCoordinate) && currentCoordinate == coordinate)
					return true;

			return false;
		}
	}
}
