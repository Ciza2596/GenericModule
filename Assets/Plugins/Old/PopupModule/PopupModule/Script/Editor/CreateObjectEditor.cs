using UnityEditor;
using UnityEngine;

namespace CizaPopupModule.Editor
{
    public class CreateObjectEditor
    {
        public const string PopupModulePath = "PopupModule/";

        public const string Popup = "Popup";

        [MenuItem("GameObject/Ciza/PopupModule/Popup", false, -10)]
        public static void CreatePopup()
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