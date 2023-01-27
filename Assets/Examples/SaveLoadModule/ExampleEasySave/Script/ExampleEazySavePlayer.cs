using System;
using UnityEngine;


[Serializable]
public class ExampleEazySavePlayer
{
    [SerializeField]
    private int _hp;

    public void SetHp(int hp) => _hp = hp;
}