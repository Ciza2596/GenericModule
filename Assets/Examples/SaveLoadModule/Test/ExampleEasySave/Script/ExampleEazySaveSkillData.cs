using System;


[Serializable]
public class ExampleEazySaveSkillData
{
    //public variable
    [ES3Serializable] public string Name { get; private set; } = "DefaultAttack";
    public int MaxUseCount { get; } = 10;
    [ES3Serializable] public int UseCount { get; private set; } = 8;
}