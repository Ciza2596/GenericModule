using UnityEditor;
using UnityEngine;

namespace CizaInputModule.Editor
{
    public class CreateObjectEditor
    {
        public const string InputModulePath = "InputModule/";

        public const string PlayerInputManager = "PlayerInputManager";

        [MenuItem("GameObject/Ciza/InputModule/PlayerInputManager", false, -101)]
        public static void CreatePlayerInputManager()
        {
            CreateObject(PlayerInputManager);
        }

        public const string PlayerInput = "PlayerInput";

        [MenuItem("GameObject/Ciza/InputModule/PlayerInput", false, -100)]
        public static void CreatePlayerInput()
        {
            CreateObject(PlayerInput);
        }

        public const string EventSystem = "EventSystem";

        [MenuItem("GameObject/Ciza/InputModule/EventSystem", false, -10)]
        public static void CreateEventSystem()
        {
            CreateObject(EventSystem);
        }

        private static void CreateObject(string dataId)
        {
            var prefab = Resources.Load<GameObject>(InputModulePath + dataId);
            var uiObject = Object.Instantiate(prefab, Selection.activeTransform);
            uiObject.name = dataId;
        }
    }
}