using System;

namespace CizaInputModule
{
    public static class RebindActionUIExtension
    {
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