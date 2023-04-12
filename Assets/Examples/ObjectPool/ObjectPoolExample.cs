using System;
using CizaObjectPool;
using UnityEngine;

public class ObjectPoolExample : MonoBehaviour
{
    private readonly ObjectPool<Character> _characterPool = new ObjectPool<Character>();

    // unity callback
    private void Awake()
    {
        var character = _characterPool.Spawn();
        character.Initialize("Ciza", 100);
        
        _characterPool.DeSpawn(character); // call Dispose method
    }

    private void OnApplicationQuit()
    {
        if(_characterPool.HasPool)
            _characterPool.Release();
    }

    private class Character : IDisposable
    {
        public string Name { get; private set; }
        public float Hp { get; private set; }
        public GameObject Body { get; private set; }

        public void Initialize(string name, float hp)
        {
            Name = name;
            Hp = hp;
            Body = new GameObject(Name);
        }
        
        public void Dispose()
        {
            Name = string.Empty;
            Hp = 0;
            var body = Body;
            Body = null;
            Destroy(body);
        }
    }
}