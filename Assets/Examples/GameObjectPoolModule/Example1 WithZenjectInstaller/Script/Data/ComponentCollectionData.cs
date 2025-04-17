using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace CizaGameObjectPoolModule.Example1
{
    [Serializable]
    public class ComponentCollectionData
    {
        [SerializeField]
        private PrefabMapData[] _prefabMapDatas;

        [Space]
        [SerializeField] private Transform _gameObjectParentTransform;
        [SerializeField]private Vector3 _gameObjectPosition;
        
        [Space] [SerializeField] private TMP_InputField _inputField;

        [Space] [SerializeField] private Button _spawnButton;
        [SerializeField] private Button _despawnButton;
        [SerializeField] private Button _despawnAllButton;
        [SerializeField] private Button _releasePoolButton;

        public Dictionary<string, GameObject> GetPrefabMap()
        {
            var prefabMap = new Dictionary<string, GameObject>();
            foreach (var prefabMapData in _prefabMapDatas)
                prefabMap.Add(prefabMapData.Key, prefabMapData.Prefab);

            return prefabMap;
        }

        public Vector3 GameObjectPosition => _gameObjectPosition;
        public Transform GameObjectParentTransform => _gameObjectParentTransform;
        
        
        public string Key => _inputField.text;
        public Button SpawnButton => _spawnButton;
        public Button DespawnButton => _despawnButton;
        public Button DespawnAllButton => _despawnAllButton;

        public Button ReleasePoolButton => _releasePoolButton;
    }
}