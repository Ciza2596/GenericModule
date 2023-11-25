using System;
using CizaCore;
using UnityEngine;

namespace CizaOptionModule
{
	public abstract class Option : MonoBehaviour, IOptionReadModel
	{
		private OptionModule _optionModule;

		// PlayerIndex, OptionKey, IsUnlock
		private event Action<int, string, bool> _onConfirm;

		// PlayerIndex, OptionKey
		private event Action<int, string> _onPointerEnter;

		public event Action<object[]> OnInitialize;

		// OptionKey
		public event Action<string> OnSelect;

		// OptionKey
		public event Action<string> OnUnselect;

		// OptionKey
		public event Action<string, bool> OnConfirm;

		// OptionKey
		public event Action<string> OnPointerEnter;

		// OptionKey, IsNew
		public event Action<string, bool> OnIsNew;

		public int PlayerIndex { get; protected set; }

		public string Key      { get; protected set; }
		public bool   IsEnable { get; protected set; }
		public bool   IsUnlock { get; protected set; }
		public bool   IsNew    { get; protected set; }

		public virtual void Initialize(OptionModule optionModule, int playerIndex, string key, bool isEnable, bool isUnlock, bool isNew, Action<int, string, bool> onConfirm, Action<int, string> onPointerEnter, object[] parameters)
		{
			_optionModule = optionModule;

			PlayerIndex = playerIndex;
			Key         = key;
			IsEnable    = isEnable;
			IsUnlock    = isUnlock;
			IsNew       = isNew;

			_onConfirm      = onConfirm;
			_onPointerEnter = onPointerEnter;

			ClearEvents();
			foreach (var optionSubMon in gameObject.GetComponentsInChildren<IOptionSup>())
				optionSubMon.Initialize(this);

			OnInitialize?.Invoke(parameters);
			OnIsNew?.Invoke(Key, IsNew);
		}

		public virtual void Select(bool isAutoTurnOffIsNew)
		{
			OnSelect?.Invoke(Key);

			if (isAutoTurnOffIsNew)
				IsNew = false;
		}

		public virtual void Unselect()
		{
			OnUnselect?.Invoke(Key);
			OnIsNew?.Invoke(Key, IsNew);
		}

		public virtual bool TryConfirm()
		{
			if (_optionModule != null)
				return _optionModule.TryConfirm(PlayerIndex);

			return TryConfirm(PlayerIndex, Key, IsUnlock);
		}

		public virtual bool TryConfirm(int playerIndex) =>
			TryConfirm(playerIndex, Key, IsUnlock);

		public virtual void PointerEnter()
		{
			if (_optionModule != null)
				_optionModule.TrySetCurrentCoordinate(PlayerIndex, Key, false);

			else
				PointerEnter(PlayerIndex, Key);
		}

		protected virtual bool TryConfirm(int playerIndex, string key, bool isUnlock)
		{
			if (!IsEnable)
				return false;

			_onConfirm?.Invoke(playerIndex, key, isUnlock);
			OnConfirm?.Invoke(key, isUnlock);
			return true;
		}

		protected virtual void PointerEnter(int playerIndex, string key)
		{
			_onPointerEnter?.Invoke(playerIndex, key);
			OnPointerEnter?.Invoke(key);
		}

		private void ClearEvents()
		{
			OnInitialize = null;

			OnSelect   = null;
			OnUnselect = null;

			OnConfirm = null;

			OnIsNew = null;
		}
	}
}
