using System;
using CizaCore;
using UnityEngine;

namespace CizaOptionModule
{
	public abstract class Option : MonoBehaviour, IOptionReadModel
	{
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

		public virtual void Initialize(int playerIndex, string key, bool isEnable, bool isUnlock, bool isNew, Action<int, string, bool> onConfirm, Action<int, string> onPointerEnter, object[] parameters)
		{
			PlayerIndex = playerIndex;
			Key         = key;
			IsEnable    = isEnable;
			IsUnlock    = isUnlock;
			IsNew       = isNew;

			_onConfirm      = onConfirm;
			_onPointerEnter = onPointerEnter;

			foreach (var optionSubMon in gameObject.GetComponentsInChildren<IOptionSubMon>())
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

		public virtual bool TryConfirm() =>
			TryConfirm(PlayerIndex);

		public virtual bool TryConfirm(int playerIndex)
		{
			if (!IsEnable)
				return false;

			_onConfirm?.Invoke(playerIndex, Key, IsUnlock);
			OnConfirm?.Invoke(Key, IsUnlock);
			return true;
		}

		public virtual void PointerEnter()
		{
			_onPointerEnter?.Invoke(PlayerIndex, Key);
			OnPointerEnter?.Invoke(Key);
		}
	}
}
