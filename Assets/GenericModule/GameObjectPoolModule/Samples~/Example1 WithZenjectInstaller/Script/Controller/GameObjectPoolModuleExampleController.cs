using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace GameObjectPoolModule.Example1
{
    public class GameObjectPoolModuleExampleController : IInitializable
    {
        private GameObjectPoolModule _gameObjectPoolModule;
        private ComponentCollectionData _componentCollectionData;
        
        private readonly List<GameObject> _gameObjects = new List<GameObject>();
        private GameObject _currentGameObject;

        public GameObjectPoolModuleExampleController(GameObjectPoolModule gameObjectPoolModule,
            ComponentCollectionData componentCollectionData)
        {
            _componentCollectionData = componentCollectionData;
            _gameObjectPoolModule = gameObjectPoolModule;
            _gameObjectPoolModule.Initialize(_componentCollectionData.GameObjectResourceDatas);
        }


        public void Initialize()
        {
            _componentCollectionData.SpawnButton.onClick.AddListener(() =>
            {
                var key = _componentCollectionData.Key;
                var gameObjectPosition = _componentCollectionData.GameObjectPosition;
                var gameObjectParentTransform = _componentCollectionData.GameObjectParentTransform;
                _currentGameObject = _gameObjectPoolModule.Spawn(key,gameObjectPosition, gameObjectParentTransform);
                _gameObjects.Add(_currentGameObject);
            });

            _componentCollectionData.DeSpawnButton.onClick.AddListener(() =>
            {
                _gameObjects.Remove(_currentGameObject);
                _gameObjectPoolModule.DeSpawn(_currentGameObject);
            });
            
            _componentCollectionData.DeSpawnAllButton.onClick.AddListener(() =>
            {
                var gameObjects = _gameObjects.ToArray();
                _gameObjects.Clear();
                _gameObjectPoolModule.DeSpawn(gameObjects);
            });
            
            _componentCollectionData.ReleasePoolButton.onClick.AddListener(() =>
            {
                _gameObjectPoolModule.ReleasePool(_componentCollectionData.Key);
            });
        }
    }
}