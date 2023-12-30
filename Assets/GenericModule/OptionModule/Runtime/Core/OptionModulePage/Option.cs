using System;
using CizaCore;
using UnityEngine;

namespace CizaOptionModule
{
    public abstract class Option : MonoBehaviour, IOptionReadModel
    {
        protected OptionModule OptionModule { get; private set; }

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

        public int PlayerIndex { get; private set; }
        public string Key { get; private set; }

        public bool IsEnable { get; private set; }
        public bool IsUnlock { get; private set; }
        public bool IsNew { get; private set; }

        public virtual void Initialize(OptionModule optionModule, int playerIndex, string key, bool isEnable, bool isUnlock, bool isNew, Action<int, string, bool> onConfirm, Action<int, string> onPointerEnter, object[] parameters)
        {
            SetOptionModule(optionModule);

            SetPlayerIndex(playerIndex);
            SetKey(key);

            SetIsEnable(isEnable);
            SetIsUnlock(isUnlock);
            SetIsNew(isNew);

            _onConfirm = onConfirm;
            _onPointerEnter = onPointerEnter;

            foreach (var optionSup in gameObject.GetComponentsInChildren<IOptionSup>())
            {
                if (optionSup.IsInitialized)
                    optionSup.Release(this);

                optionSup.Initialize(this);
            }

            OnInitialize?.Invoke(parameters);
            OnIsNew?.Invoke(Key, IsNew);
        }

        public virtual void SetOptionModule(OptionModule optionModule) =>
            OptionModule = optionModule;

        public virtual void SetPlayerIndex(int playerIndex) =>
            PlayerIndex = playerIndex;

        public virtual void SetKey(string key) =>
            Key = key;

        public virtual void SetIsEnable(bool isEnable) =>
            IsEnable = isEnable;

        public virtual void SetIsUnlock(bool isUnlock) =>
            IsUnlock = isUnlock;

        public virtual void SetIsNew(bool isNew) =>
            IsNew = isNew;

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
            if (OptionModule != null)
                return OptionModule.TryConfirm(PlayerIndex);

            return TryConfirm(PlayerIndex, Key, IsUnlock);
        }

        public virtual bool TryConfirm(int playerIndex) =>
            TryConfirm(playerIndex, Key, IsUnlock);

        public virtual void PointerEnter()
        {
            if (OptionModule != null)
                OptionModule.TrySetCurrentCoordinate(PlayerIndex, Key, false);

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
    }
}