using UnityEngine;

public class ExampleEasySave : MonoBehaviour
{
    [SerializeField] private string _path = "Hello.text";
    [SerializeField] private string _key = "player";
    [SerializeField] private ExampleEazySavePlayerData _playerData;

    public void OnEnable()
    {
        ES3.Save(_key, _playerData, _path);
    }
}