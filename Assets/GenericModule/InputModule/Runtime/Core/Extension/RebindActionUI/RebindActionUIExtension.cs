using System;
using UnityEngine.InputSystem;

namespace CizaInputModule
{
    public static class RebindActionUIExtension
    {
        public static bool TryGetActionAndBindingIndex(this InputActionMap inputActionMap, string path, out InputAction inputAction, out int bindingIndex)
        {
            foreach (var action in inputActionMap.actions)
                foreach (var binding in action.bindings)
                {
                    if (binding.path == path)
                    {
                        inputAction = action;
                        bindingIndex = action.bindings.IndexOf(x => x.id == binding.id);
                        return true;
                    }
                }

            inputAction = null;
            bindingIndex = -1;
            return false;
        }

        public static void RebindActionsByJson(this RebindActionUI[] rebindActionUIs, string json)
        {
            foreach (var rebindActionUI in rebindActionUIs)
                rebindActionUI.RebindActionsByJson(json);
        }

        public static void ResetToDefault(this RebindActionUI[] rebindActionUIs)
        {
            foreach (var rebindActionUI in rebindActionUIs)
                rebindActionUI.ResetToDefault();
        }

        public static void Initialize(this RebindActionUI[] rebindActionUIs, string json, string[] excludingPaths, Action<string, string, string> onRebindActionStarted, Action<string, string, string> onRebindActionCompleted, Action<string, string, string> onRebindActionCancel)
        {
            rebindActionUIs.Initialize(excludingPaths, onRebindActionStarted, onRebindActionCompleted, onRebindActionCancel);
            rebindActionUIs.RebindActionsByJson(json);
        }

        public static void Initialize(this RebindActionUI[] rebindActionUIs, string[] excludingPaths, Action<string, string, string> onRebindActionStarted, Action<string, string, string> onRebindActionCompleted, Action<string, string, string> onRebindActionCancel)
        {
            foreach (var rebindActionUI in rebindActionUIs)
            {
                rebindActionUI.OnRebindActionStarted += onRebindActionStarted;
                rebindActionUI.OnRebindActionCompleted += onRebindActionCompleted;
                rebindActionUI.OnRebindActionCancel += onRebindActionCancel;

                rebindActionUI.SetExcludingPaths(excludingPaths);
            }

            rebindActionUIs.ResetToDefault();
        }

        public static void Release(this RebindActionUI[] rebindActionUIs, Action<string, string, string> onRebindActionStarted, Action<string, string, string> onRebindActionCompleted, Action<string, string, string> onRebindActionCancel)
        {
            foreach (var rebindActionUI in rebindActionUIs)
            {
                rebindActionUI.OnRebindActionStarted -= onRebindActionStarted;
                rebindActionUI.OnRebindActionCompleted -= onRebindActionCompleted;
                rebindActionUI.OnRebindActionCancel -= onRebindActionCancel;
            }
        }
    }
}