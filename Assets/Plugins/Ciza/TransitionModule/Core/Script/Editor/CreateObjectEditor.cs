using UnityEditor;
using UnityEngine;

namespace CizaTransitionModule.Editor
{
    public class CreateObjectEditor
    {
        public const string TransitionModulePath = "TransitionModule/";

        public const string TransitionInPage = "TransitionInPage";

        [MenuItem("GameObject/Ciza/TransitionModule/TransitionInPage", false, -10)]
        public static void CreateTransitionInPage()
        {
            CreateObject(TransitionInPage);
        }

        public const string LoadingPage = "LoadingPage";

        [MenuItem("GameObject/Ciza/TransitionModule/LoadingPage", false, -9)]
        public static void CreateLoadingPage()
        {
            CreateObject(LoadingPage);
        }

        public const string TransitionOutPage = "TransitionOutPage";

        [MenuItem("GameObject/Ciza/TransitionModule/TransitionOutPage", false, -8)]
        public static void CreateTransitionOutPage()
        {
            CreateObject(TransitionOutPage);
        }

        private static void CreateObject(string dataId)
        {
            var prefab = Resources.Load<GameObject>(TransitionModulePath + dataId);
            var uiObject = Object.Instantiate(prefab, Selection.activeTransform);
            uiObject.name = dataId;
        }
    }
}