using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CizaInputModule
{
    /// <summary>
    /// A reusable component with a self-contained UI for rebinding a single action.
    /// </summary>
    public class RebindActionUI : MonoBehaviour
    {
        [SerializeField]
        private InputActionReference _inputActionReference;

        [SerializeField]
        private string _bindingId;


        private string[] _excludingPaths;
        private InputActionRebindingExtensions.RebindingOperation _rebindOperation;


        // ActionMapDataId ex.Menu, ActionDataId ex.Movement, Path ex.<KeyBoard>/w
        public event Action<string, string, string> OnRebindActionStarted;

        public event Action<string, string, string> OnRebindActionCompleted;

        public event Action<string, string, string> OnRebindActionCancel;

        public string ActionMapDataId
        {
            get
            {
                if (!TryGetInputActionAndBindingIndex(out var actionMap, out var action, out var bindingIndex))
                    return string.Empty;

                return actionMap.name;
            }
        }

        public string ActionDataId
        {
            get
            {
                if (!TryGetInputActionAndBindingIndex(out var actionMap, out var action, out var bindingIndex))
                    return string.Empty;

                return action.name;
            }
        }

        public string Path
        {
            get
            {
                if (!TryGetInputActionAndBindingIndex(out var actionMap, out var action, out var bindingIndex))
                    return string.Empty;

                return action.bindings[bindingIndex].path;
            }
        }

        public string[] ExcludingPaths => _excludingPaths != null ? _excludingPaths : Array.Empty<string>();


        public void SetExcludingPaths(string[] paths) =>
            _excludingPaths = paths;


        public void RebindActionsByJson(string json) =>
            _inputActionReference.asset.LoadBindingOverridesFromJson(json);

        /// <summary>
        /// Remove currently applied binding overrides.
        /// </summary>
        public void ResetToDefault()
        {
            if (!TryGetInputActionAndBindingIndex(out var actionMap, out var action, out var bindingIndex))
                return;

            if (action.bindings[bindingIndex].isComposite)
                for (var i = bindingIndex + 1; i < action.bindings.Count && action.bindings[i].isPartOfComposite; ++i)
                    action.RemoveBindingOverride(i);

            else
                action.RemoveBindingOverride(bindingIndex);
        }

        /// <summary>
        /// Initiate an interactive rebind that lets the player actuate a control to choose a new binding
        /// for the action.
        /// </summary>
        public void StartRebind()
        {
            if (!TryGetInputActionAndBindingIndex(out var actionMap, out var action, out var bindingIndex))
                return;

            if (action.bindings[bindingIndex].isComposite)
            {
                var firstPartIndex = bindingIndex + 1;
                if (firstPartIndex < action.bindings.Count && action.bindings[firstPartIndex].isPartOfComposite)
                    PerformInteractiveRebind(action, firstPartIndex, allCompositeParts: true);
            }
            else
                PerformInteractiveRebind(action, bindingIndex);
        }

        /// <summary>
        /// Return the action and binding index for the binding that is targeted by the component
        /// according to
        /// </summary>
        /// <param name="actionMap"></param>
        /// <param name="action"></param>
        /// <param name="bindingIndex"></param>
        /// <returns></returns>
        private bool TryGetInputActionAndBindingIndex(out InputActionMap actionMap, out InputAction action, out int bindingIndex)
        {
            bindingIndex = -1;
            actionMap = null;
            action = _inputActionReference?.action;
            if (action == null)
                return false;

            actionMap = action.actionMap;
            if (actionMap == null)
                return false;

            if (string.IsNullOrEmpty(_bindingId))
                return false;

            // Look up binding index.
            var bindingId = new Guid(_bindingId);
            bindingIndex = action.bindings.IndexOf(x => x.id == bindingId);
            if (bindingIndex == -1)
            {
                Debug.LogError($"Cannot find binding with ID '{bindingId}' on '{action}'", this);
                return false;
            }

            return true;
        }

        private void PerformInteractiveRebind(InputAction action, int bindingIndex, bool allCompositeParts = false)
        {
            _rebindOperation?.Cancel(); // Will null out m_RebindOperation.

            void m_Clear()
            {
                _rebindOperation?.Dispose();
                _rebindOperation = null;
            }

            // Configure the rebind.
            _rebindOperation = action.PerformInteractiveRebinding(bindingIndex)
                .OnCancel(
                    operation =>
                    {
                        OnRebindActionCancel?.Invoke(ActionMapDataId, ActionDataId, Path);
                        m_Clear();
                    })
                .OnComplete(
                    operation =>
                    {
                        OnRebindActionCompleted?.Invoke(ActionMapDataId, ActionDataId, Path);
                        m_Clear();

                        // If there's more composite parts we should bind, initiate a rebind
                        // for the next part.
                        if (allCompositeParts)
                        {
                            var nextBindingIndex = bindingIndex + 1;
                            if (nextBindingIndex < action.bindings.Count && action.bindings[nextBindingIndex].isPartOfComposite)
                                PerformInteractiveRebind(action, nextBindingIndex, true);
                        }
                    });

            foreach (var excludingPath in ExcludingPaths)
                _rebindOperation.WithControlsExcluding(excludingPath);

            OnRebindActionStarted?.Invoke(ActionMapDataId, ActionDataId, Path);
            _rebindOperation.Start();
        }
    }
}