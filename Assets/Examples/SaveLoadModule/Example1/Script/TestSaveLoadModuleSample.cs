using System.Collections.Generic;
using UnityEngine;


namespace SaveLoadModule.Example1
{
    public class TestSaveLoadModuleSample : MonoBehaviour
    {
        [SerializeField] private string _key;
        [SerializeField] private string _filePath;
        [Space] [SerializeField] private float _hp;

        [Space] public List<GameObjectMap> _gameObjectMaps;


        private PlayerData _playerData;

        private SaveLoadModule _saveLoadModule;

        //unity callback
        private void Awake() =>
            _saveLoadModule = new SaveLoadModule();


        private void OnEnable()
        {
            var playerData = new PlayerData();
            playerData.SetHp(_hp);

            foreach (var gameObjectMap in _gameObjectMaps)
            {
                var key = gameObjectMap.Key;
                var prefab = gameObjectMap.Prefab;

                playerData.AddGameObjectDict(key, prefab);
            }

            _saveLoadModule.Save<PlayerData>(_key, _playerData, _filePath);
        }

        private void OnDisable()
        {
            _playerData = _saveLoadModule.Load<PlayerData>(_key, _filePath);
            
            Print($"Hp: {_playerData.Hp}");
            
            foreach (var keyValuePair in _playerData.GameObjectDict)
            {
                var key = keyValuePair.Key;
                var value = keyValuePair.Value;
                Print($"Key: {key}, Prefab: {value}");
            }
        }
        
        
        //private method
        private void Print(string message) =>
            Debug.Log(message);
        
    }
}