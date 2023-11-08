using System;
using CizaCore;
using UnityEngine;

namespace CizaOptionModule
{
	public abstract class Option : MonoBehaviour, IOptionReadModel
	{
		private event Action<string, bool> _onConfirm;
		private event Action<string>       _onPointerEnter;

		public event Action<object[]> OnInitialize;

		public event Action<string>       OnSelect;
		public event Action<string>       OnUnselect;
		public event Action<string, bool> OnConfirm;
		public event Action<string>       OnPointerEnter;
		public event Action<string, bool> OnIsNew;

		public string Key      { get; protected set; }
		public bool   IsEnable { get; protected set; }
		public bool   IsUnlock { get; protected set; }
		public bool   IsNew    { get; protected set; }

		public virtual void Initialize(string key, bool isEnable, bool isUnlock, bool isNew, Action<string, bool> onConfirm, Action<string> onPointerEnter, object[] parameters)
		{
			Key      = key;
			IsEnable = isEnable;
			IsUnlock = isUnlock;
			IsNew    = isNew;

			_onConfirm      = onConfirm;
			_onPointerEnter = onPointerEnter;

			var optionSubMons = gameObject.GetComponentsInChildren<IOptionSubMon>();
			foreach (var optionSubMon in optionSubMons)
				optionSubMon.Initialize(this);

			OnInitialize?.Invoke(parameters);
			OnIsNew?.Invoke(Key, IsNew);
		}

		public virtual void Select(bool isAutoTurnOffIsNew)
		{
			OnSelect?.Invoke(Key);
			
			if(isAutoTurnOffIsNew) 
				IsNew = false;
		}

		public virtual void Unselect()
		{
			OnUnselect?.Invoke(Key);
			OnIsNew?.Invoke(Key, IsNew);
		}

		public virtual bool TryConfirm()
		{
			if (!IsEnable)
				return false;

			_onConfirm?.Invoke(Key, IsUnlock);
			OnConfirm?.Invoke(Key, IsUnlock);

			return true;
		}

		public virtual void PointerEnter()
		{
			_onPointerEnter?.Invoke(Key);
			OnPointerEnter?.Invoke(Key);
		}
	}
}
