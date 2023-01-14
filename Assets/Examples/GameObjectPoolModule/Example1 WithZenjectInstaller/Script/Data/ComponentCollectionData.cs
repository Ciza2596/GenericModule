using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace GameObjectPoolModule.Example1
{
    [Serializable]
    public class ComponentCollectionData
    {
        [SerializeField] private GameObjectResourceData[] _gameObjectResourceDatas;

        [Space]
        [SerializeField] private Transform _gameObjectParentTransform;
        [SerializeField]private Vector3 _gameObjectPosition;
        
        [Space] [SerializeField] private TMP_InputField _inputField;

        [Space] [SerializeField] private Button _spawnButton;
        [SerializeField] private Button _deSpawnButton;
        [SerializeField] private Button _deSpawnAllButton;
        [SerializeField] private Button _releasePoolButton;

        public IGameObjectResourceData[] GameObjectResourceDatas => _gameObjectResourceDatas;

        public Vector3 GameObjectPosition => _gameObjectPosition;
        public Transform GameObjectParentTransform => _gameObjectParentTransform;
        
        
        public string Key => _inputField.text;
        public Button SpawnButton => _spawnButton;
        public Button DeSpawnButton => _deSpawnButton;
        public Button DeSpawnAllButton => _deSpawnAllButton;

        public Button ReleasePoolButton => _releasePoolButton;
    }
}