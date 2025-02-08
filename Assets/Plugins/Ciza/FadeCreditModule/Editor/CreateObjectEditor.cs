using UnityEditor;
using UnityEngine;

namespace CizaFadeCreditModule.Editor
{
    public class CreateObjectEditor
    {
        public const string FadeCreditModulePath = "FadeCreditModule/";

        public const string FadeCreditController = "FadeCreditController";

        [MenuItem("GameObject/Ciza/FadeCreditModule/FadeCreditController", false, -10)]
        public static void CreateFadeCreditController()
        {
            CreateObject(FadeCreditController);
        }

        public const string FadeCreditRow = "FadeCreditRow";

        [MenuItem("GameObject/Ciza/FadeCreditModule/FadeCreditRow", false, -10)]
        public static void CreateFadeCreditRow()
        {
            CreateObject(FadeCreditRow);
        }

        private static void CreateObject(string dataId)
        {
            var prefab = Resources.Load<GameObject>(FadeCreditModulePath + dataId);
            var uiObject = Object.Instantiate(prefab, Selection.activeTransform);
            uiObject.name = dataId;
        }
    }
}