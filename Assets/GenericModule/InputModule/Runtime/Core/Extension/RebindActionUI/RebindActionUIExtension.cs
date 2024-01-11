using System;
using UnityEngine.InputSystem;

namespace CizaInputModule
{
    public static class RebindActionUIExtension
    {
        public static bool TryGetActionAndBindingIndex(this InputActionMap inputActionMap, string path, string bindingId, out InputAction otherInputAction, out int otherBindingIndex) =>
            inputActionMap.TryGetActionAndBindingIndex(path, new Guid(bindingId), out otherInputAction, out otherBindingIndex);

        public static bool TryGetActionAndBindingIndex(this InputActionMap inputActionMap, string path, Guid bindingId, out InputAction otherInputAction, out int otherBindingIndex)
        {
            foreach (var action in inputActionMap.actions)
                foreach (var binding in action.bindings)
                {
                    if (binding.effectivePath == path && binding.id != bindingId && !binding.isPartOfComposite)
                    {
                        otherBindingIndex = action.bindings.IndexOf(x => x.id == binding.id);
                        otherInputAction = action;
                        return true;
                    }
                }

            otherInputAction = null;
            otherBindingIndex = -1;
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