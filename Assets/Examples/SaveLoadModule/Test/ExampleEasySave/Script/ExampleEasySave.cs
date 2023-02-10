using Sirenix.OdinInspector;
using UnityEngine;

public class ExampleEasySave : MonoBehaviour
{
    [SerializeField] private string _path = "Hello.text";
    [SerializeField] private string _key = "player";
    [SerializeField] private ExampleEazySavePlayerData _playerData;

    [Button]
    public void Save()
    {
        ES3.Save(_key, _playerData, _path);
    }

    [Button]
    public void Load()
    {
        _playerData = ES3.Load<ExampleEazySavePlayerData>(_key, _path);
    }
}