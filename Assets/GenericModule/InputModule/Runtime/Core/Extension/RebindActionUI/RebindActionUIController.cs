using System;
using System.Collections.Generic;
using System.Linq;

namespace CizaInputModule
{
    public class RebindActionUIController
    {
        private readonly Dictionary<string, RebindActionUI> _rebindActionUIMapByKey = new Dictionary<string, RebindActionUI>();

        // ActionMapDataId ex.Menu, ActionDataId ex.Movement, Path ex.<KeyBoard>/w
        public event Action<string, string, string> OnRebindActionStarted;
        public event Action<string, string, string> OnRebindActionEnd;

        public event Action<string, string, string> OnRebindActionCompleted;
        public event Action<string, string, string> OnRebindActionCancel;

        public bool IsInitialized { get; private set; }

        public string[] ExcludingPaths
        {
            get
            {
                if (!IsInitialized || _rebindActionUIMapByKey.Count <= 0)
                    return Array.Empty<string>();

                return _rebindActionUIMapByKey.First().Value.ExcludingPaths;
            }
        }

        public void AddRebindActionUI(string key, RebindActionUI rebindActionUI)
        {
            if (IsInitialized)
                return;

            _rebindActionUIMapByKey.Add(key, rebindActionUI);
        }

        public void Initialize()
        {
            if (IsInitialized)
                return;

            IsInitialized = true;

            foreach (var rebindActionUI in _rebindActionUIMapByKey.Values.ToArray())
            {
                rebindActionUI.OnRebindActionStarted += OnRebindActionStartedImp;
                rebindActionUI.OnRebindActionEnd += OnRebindActionEndImp;

                rebindActionUI.OnRebindActionCompleted += OnRebindActionCompletedImp;
                rebindActionUI.OnRebindActionCancel += OnRebindActionCancelImp;
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
                _rebindActionUIMapByKey.Remove(pair.Key);
            }
        }

        public void SetDefaultExcludingPaths() =>
            SetExcludingPaths(RebindActionUIUtils.AllPaths());

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


        private bool TryGetRebindActionUIWithIsInitialized(string key, out RebindActionUI rebindActionUI) =>
            _rebindActionUIMapByKey.TryGetValue(key, out rebindActionUI) && IsInitialized;


        // ActionMapDataId ex.Menu, ActionDataId ex.Movement, Path ex.<KeyBoard>/w
        private void OnRebindActionStartedImp(string actionMapDataId, string actionDataId, string path) =>
            OnRebindActionStarted?.Invoke(actionMapDataId, actionDataId, path);

        private void OnRebindActionEndImp(string actionMapDataId, string actionDataId, string path) =>
            OnRebindActionEnd?.Invoke(actionMapDataId, actionDataId, path);

        private void OnRebindActionCompletedImp(string actionMapDataId, string actionDataId, string path) =>
            OnRebindActionCompleted?.Invoke(actionMapDataId, actionDataId, path);

        private void OnRebindActionCancelImp(string actionMapDataId, string actionDataId, string path) =>
            OnRebindActionCancel?.Invoke(actionMapDataId, actionDataId, path);
    }
}