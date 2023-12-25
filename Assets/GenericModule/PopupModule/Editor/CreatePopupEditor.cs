using UnityEditor;
using UnityEngine;

namespace CizaPopupModule.Editor
{
    public class CreatePopupEditor
    {
        public const string PopupModulePath = "PopupModule/";
        
        public const string PopupCanvas = "PopupCanvas";
        
        [MenuItem("GameObject/Ciza/PopupModule/PopupCanvas", false, 10)]
        public static void CreateVerticalScrollView()
        {
            CreateUIObject(PopupCanvas);
        }
        
        public const string Popup = "Popup";
        
        [MenuItem("GameObject/Ciza/PopupModule/Popup", false, 10)]
        public static void CreateDropdown()
        {
            CreateUIObject(Popup);
        }
        
        private static void CreateUIObject(string dataId)
        {
            var prefab = Resources.Load<GameObject>(PopupModulePath + dataId);
            var uiObject = Object.Instantiate(prefab, Selection.activeTransform);
            uiObject.name = dataId;
        }
    }
}