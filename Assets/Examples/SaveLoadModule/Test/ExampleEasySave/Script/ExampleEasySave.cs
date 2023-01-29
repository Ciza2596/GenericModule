using System;
using UnityEngine;

public class ExampleEasySave : MonoBehaviour
{
    [SerializeField] private string _key = "player";
    [SerializeField] private ExampleEazySavePlayer _player;

    public void OnEnable()
    {
        ES3.Save<ExampleEazySavePlayer>(_key, _player);
    }
}