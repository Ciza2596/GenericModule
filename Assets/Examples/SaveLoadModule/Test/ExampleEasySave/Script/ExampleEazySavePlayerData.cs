using System;
using UnityEngine;


[Serializable]
public class ExampleEazySavePlayerData
{
    [SerializeField]
    private int _hp;

    [ES3Serializable] private ExampleEazySaveSkillData _skillData;

    public void SetHp(int hp) => _hp = hp;
}