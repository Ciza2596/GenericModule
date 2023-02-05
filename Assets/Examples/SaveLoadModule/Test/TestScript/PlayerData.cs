using System;
using System.Collections.Generic;
using UnityEngine;

namespace SaveLoadModule.Example2
{
    [Serializable]
    public class PlayerData
    {
        //private variable
        [SerializeField] private float _hp;
        [SerializeField] private Dictionary<string, GameObject> _gameObjectDict;


        //public variable
        public float Hp => _hp;
        public IReadOnlyDictionary<string, GameObject> GameObjectDict => _gameObjectDict;

        
        //public method
        public void SetHp(float hp) => _hp = hp;
        public void AddGameObjectDict(string key, GameObject gameObject) => _gameObjectDict.Add(key, gameObject);
    }
}