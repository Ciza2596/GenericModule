using System;
using UnityEngine;
using UnityEngine.UI;

namespace SceneModule.Example1
{
    [Serializable]
    public class ComponentCollection
    {
        [SerializeField] private Button _goToSceneButton;

        public Button GoToSceneButton => _goToSceneButton;
    }
}