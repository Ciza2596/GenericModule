using System.IO;
using UnityEditor;
using UnityEngine;

namespace CizaInputModule.Editor
{
    public static class CreateObjectMenu
    {
        public const string PROJECT_ID = "InputModule";
        public const string ROOT_PATH = "Prefab";

        public const string PLAYER_INPUT_MANAGER = "PlayerInputManager";

        [MenuItem("GameObject/Ciza/InputModule/PlayerInputManager", false, -101)]
        public static void CreatePlayerInputManager()
        {
            CreateObject(PLAYER_INPUT_MANAGER);
        }

        public const string PLAYER_INPUT = "PlayerInput";

        [MenuItem("GameObject/Ciza/InputModule/PlayerInput", false, -100)]
        public static void CreatePlayerInput()
        {
            CreateObject(PLAYER_INPUT);
        }

        public const string EVENT_SYSTEM = "EventSystem";

        [MenuItem("GameObject/Ciza/InputModule/EventSystem", false, -10)]
        public static void CreateEventSystem()
        {
            CreateObject(EVENT_SYSTEM);
        }
        
        public const string VIRTUAL_MOUSE_CANVAS = "VirtualMouseCanvas";
        [MenuItem("GameObject/Ciza/InputModule/VirtualMouseCanvas", false, -9)]
        public static void CreateVirtualMouseCanvas()
        {
            CreateObject(VIRTUAL_MOUSE_CANVAS);
        }
        
        public const string VIRTUAL_MOUSE = "VirtualMouse";
        [MenuItem("GameObject/Ciza/InputModule/VirtualMouse", false, -8)]
        public static void CreateVirtualMouse()
        {
            CreateObject(VIRTUAL_MOUSE);
        }

        private static void CreateObject(string dataId)
        {
            var prefab = Resources.Load<GameObject>(Path.Combine(PROJECT_ID, ROOT_PATH, dataId));
            var uiObject = Object.Instantiate(prefab, Selection.activeTransform);
            uiObject.name = dataId;
        }
    }
}