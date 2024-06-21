using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem;

namespace CizaInputModule
{
    public class RebindActionUIController
    {
        private readonly Dictionary<string, RebindActionUI> _rebindActionUIMapByKey = new Dictionary<string, RebindActionUI>();

        // ActionMapDataId, ActionDataId, PathWithControlScheme ex.Keyboard/w
        public event Action<string, string, string> OnStart;
        public event Action<string, string, string> OnEnd;

        public event Action<string, string, string> OnComplete;
        public event Action<string, string, string> OnCancel;

        // PathWithControlScheme ex.Keyboard/w
        public event Func<string, string> OnTranslate;

        public bool IsInitialized { get; private set; }

        public bool IsAnyRebinding
        {
            get
            {
                if (!IsInitialized || _rebindActionUIMapByKey.Count <= 0)
                    return false;

                foreach (var rebindActionUI in _rebindActionUIMapByKey.Values.ToArray())
                    if (rebindActionUI.IsRebinding)
                        return true;

                return false;
            }
        }

        public string[] ExcludingPaths
        {
            get
            {
                if (!IsInitialized || _rebindActionUIMapByKey.Count <= 0)
                    return Array.Empty<string>();

                return _rebindActionUIMapByKey.First().Value.ExcludingPaths;
            }
        }

        public bool TryGetActionsJson(out string json)
        {
            if (!IsInitialized || _rebindActionUIMapByKey.Count <= 0)
            {
                json = string.Empty;
                return false;
            }

            return _rebindActionUIMapByKey.First().Value.TryGetActionsJson(out json);
        }

        public void AddRebindActionUI(string key, RebindActionUI rebindActionUI)
        {
            if (IsInitialized)
                return;

            _rebindActionUIMapByKey.Add(key, rebindActionUI);
        }

        public void Initialize(InputActionAsset asset = null)
        {
            if (IsInitialized)
                return;

            IsInitialized = true;

            foreach (var rebindActionUI in _rebindActionUIMapByKey.Values.ToArray())
            {
                rebindActionUI.SetAsset(asset);

                rebindActionUI.OnRebindActionStarted += OnRebindActionStartedImp;
                rebindActionUI.OnRebindActionEnd += OnRebindActionEndImp;

                rebindActionUI.OnRebindActionCompleted += OnRebindActionCompletedImp;
                rebindActionUI.OnRebindActionCancel += OnRebindActionCancelImp;

                rebindActionUI.OnTranslate += OnTranslateImp;
            }
        }

        public void Release()
        {
            if (!IsInitialized)
                return;

            IsInitialized = false;

            foreach (var pair in _rebindActionUIMapByKey.ToArray())
            {
                pair.Value.OnRebindActionStarted -= OnRebindActionStartedImp;
                pair.Value.OnRebindActionEnd -= OnRebindActionEndImp;

                pair.Value.OnRebindActionCompleted -= OnRebindActionCompletedImp;
                pair.Value.OnRebindActionCancel -= OnRebindActionCancelImp;

                pair.Value.OnTranslate -= OnTranslateImp;
                _rebindActionUIMapByKey.Remove(pair.Key);
            }
        }

        public void SetDefaultExcludingPaths() =>
            SetExcludingPaths(RebindActionUIUtils.GetAllExcludingPaths());

        public void SetExcludingPaths(string[] paths)
        {
            if (!IsInitialized)
                return;

            foreach (var rebindActionUI in _rebindActionUIMapByKey.Values.ToArray())
                rebindActionUI.SetExcludingPaths(paths);
        }

        public void RebindActionsByJson(string json)
        {
            if (!IsInitialized)
                return;

            foreach (var rebindActionUI in _rebindActionUIMapByKey.Values.ToArray())
                rebindActionUI.RebindActionsByJson(json);
        }

        public void ResetAllToDefault()
        {
            foreach (var key in _rebindActionUIMapByKey.Keys.ToArray())
                ResetToDefault(key);
        }

        public void ResetToDefault(string key)
        {
            if (!TryGetRebindActionUIWithIsInitialized(key, out var rebindActionUI))
                return;

            rebindActionUI.ResetToDefault();
        }

        public void StartRebind(string key)
        {
            if (!TryGetRebindActionUIWithIsInitialized(key, out var rebindActionUI))
                return;

            rebindActionUI.StartRebind();
        }

        public void CancelRebind(string key)
        {
            if (!TryGetRebindActionUIWithIsInitialized(key, out var rebindActionUI))
                return;

            rebindActionUI.CancelRebind();
        }

        public void RefreshAllText()
        {
            foreach (var key in _rebindActionUIMapByKey.Keys.ToArray())
                RefreshText(key);
        }

        public void RefreshText(string key)
        {
            if (!TryGetRebindActionUIWithIsInitialized(key, out var rebindActionUI))
                return;

            rebindActionUI.RefreshText();
        }


        private bool TryGetRebindActionUIWithIsInitialized(string key, out RebindActionUI rebindActionUI) =>
            _rebindActionUIMapByKey.TryGetValue(key, out rebindActionUI) && IsInitialized;


        private void OnRebindActionStartedImp(string actionMapDataId, string actionDataId, string pathWithControlScheme) =>
            OnStart?.Invoke(actionMapDataId, actionDataId, pathWithControlScheme);

        private void OnRebindActionEndImp(string actionMapDataId, string actionDataId, string pathWithControlScheme) =>
            OnEnd?.Invoke(actionMapDataId, actionDataId, pathWithControlScheme);

        private void OnRebindActionCompletedImp(string actionMapDataId, string actionDataId, string pathWithControlScheme)
        {
            OnComplete?.Invoke(actionMapDataId, actionDataId, pathWithControlScheme);
            RefreshAllText();
        }

        private void OnRebindActionCancelImp(string actionMapDataId, string actionDataId, string pathWithControlScheme) =>
            OnCancel?.Invoke(actionMapDataId, actionDataId, pathWithControlScheme);

        private string OnTranslateImp(string pathWithControlScheme) =>
            OnTranslate != null ? OnTranslate.Invoke(pathWithControlScheme) : string.Empty;
    }
}