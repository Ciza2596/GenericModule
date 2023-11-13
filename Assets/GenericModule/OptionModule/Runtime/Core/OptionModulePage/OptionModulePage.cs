using System;
using System.Linq;
using CizaCore;
using CizaPageModule;
using CizaPageModule.Implement;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

namespace CizaOptionModule
{
	public abstract class OptionModulePage : Page, IOptionModulePage, IInitializable, IShowingStart, IShowingAnimated, IHidingStart, IReleasable
	{
		[SerializeField]
		protected Transform _parentTransform;

		protected readonly SelectOptionLogic _selectOptionLogic = new SelectOptionLogic();

		protected IOptionView _optionView;
		protected Vector2Int  _onShowingStartCoordinate;

		protected bool _isAutoTurnOffIsNew;
		protected bool _localIsAutoTurnOffIsNew;

		public event Action<string, string> OnSelect;
		public event Action<string, bool>   OnConfirm;

		public string OptionKey => _selectOptionLogic.CurrentOptionKey;

		public int        PageIndex         { get; private set; }
		public Vector2Int CurrentCoordinate => _selectOptionLogic.CurrentCoordinate;

		public int MaxColumnIndex => _selectOptionLogic.MaxColumnLength - 1;
		public int MaxRowIndex    => _selectOptionLogic.MaxRowLength    - 1;

		public virtual UniTask InitializeAsync(params object[] parameters)
		{
			var optionModulePageInfo = parameters[0] as IOptionModulePageInfo;
			Assert.IsNotNull(optionModulePageInfo, $"[{GetType().Name}::Initialize] OptionModulePageInfo is not found.");

			PageIndex = int.Parse(optionModulePageInfo.PageIndexString);

			var optionViewGameObject = Instantiate(optionModulePageInfo.OptionViewPrefab, _parentTransform);
			_optionView = optionViewGameObject.GetComponent<IOptionView>();

			var optionInfos = parameters[1] as IOptionInfo[];
			_optionView.OptionsIncludeNull.InitializeOptions(optionModulePageInfo.OptionKeys, optionInfos, OnConfirmImp, OnPointerEnter, GetType().Name);

			_selectOptionLogic.Initialize(_optionView.OptionColumns, _optionView.Options, _optionView.ColumnInfo, _optionView.RowInfo);
			_selectOptionLogic.OnSetCurrentCoordinate += OnSetCurrentCoordinate;

			return UniTask.CompletedTask;
		}

		public virtual UniTask OnShowingStartAsync(params object[] parameters)
		{
			_onShowingStartCoordinate = (Vector2Int)parameters[0];
			_isAutoTurnOffIsNew       = (bool)parameters[1];
			return UniTask.CompletedTask;
		}

		public UniTask PlayShowingAnimationAsync()
		{
			_optionView.UnSelectAll();
			_localIsAutoTurnOffIsNew = true;
			_selectOptionLogic.TrySetCurrentCoordinate(_onShowingStartCoordinate);
			return UniTask.CompletedTask;
		}

		public virtual void OnHidingStart()
		{
			_localIsAutoTurnOffIsNew = false;
		}

		public void Release() =>
			_selectOptionLogic.OnSetCurrentCoordinate += OnSetCurrentCoordinate;

		public Option[] GetAllOptions() =>
			_optionView.Options.ToArray();

		public bool TryGetOption(string optionKey, out Option option)
		{
			option = _optionView.Options.FirstOrDefault(m_option => m_option.Key == optionKey);
			return option != null;
		}

		public bool TrySetCurrentCoordinate(Vector2Int coordinate) =>
			_selectOptionLogic.TrySetCurrentCoordinate(coordinate);

		public bool TryConfirm()
		{
			if (!_selectOptionLogic.TryGetOption(_selectOptionLogic.CurrentCoordinate, out var option))
				return false;

			return option.TryConfirm();
		}

		public virtual bool TryMoveToLeft() =>
			_selectOptionLogic.TryMoveToLeft(true);

		public virtual bool TryMoveToRight() =>
			_selectOptionLogic.TryMoveToRight(true);

		public virtual bool TryMoveToUp() =>
			_selectOptionLogic.TryMoveToUp(true);

		public virtual bool TryMoveToDown() =>
			_selectOptionLogic.TryMoveToDown(true);

		protected virtual void OnConfirmImp(string optionKey, bool isUnlock) =>
			OnConfirm?.Invoke(optionKey, isUnlock);

		protected virtual void OnPointerEnter(string optionKey) =>
			_selectOptionLogic.TrySetCurrentCoordinate(optionKey);

		protected virtual void OnSetCurrentCoordinate(Vector2Int previousCoordinate, Option previousOption, Vector2Int currentCoordinate, Option currentOption)
		{
			previousOption?.Unselect();
			currentOption?.Select(_isAutoTurnOffIsNew && _localIsAutoTurnOffIsNew);

			OnSelect?.Invoke(previousOption != null ? previousOption.Key : string.Empty, currentOption != null ? currentOption.Key : string.Empty);
		}
	}
}
