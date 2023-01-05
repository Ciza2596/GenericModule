using System;
using UnityEngine;
using UnityEngine.UI;

namespace SceneModule.Example1
{
    [Serializable]
    public class ComponentCollectionData
    {
        [SerializeField] private Button _goToSceneButton;

        public Button GoToSceneButton => _goToSceneButton;
    }
}